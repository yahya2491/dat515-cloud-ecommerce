using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;

namespace Ecommerce.Api.Services
{
    public static class CategoryPredictor
    {
        private static readonly MLContext ml = new();
        private static readonly List<string> Categories = new()
        {
            "Dairy", "Produce", "Bakery", "Beverages", "Snacks", "Frozen", "Household"
        };

        private static readonly Dictionary<string, string[]> KeywordSignals = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Dairy"] = new[] { "milk", "cheese", "butter", "yogurt", "cream", "dairy" },
            ["Produce"] = new[] { "banana", "apple", "orange", "fruit", "vegetable", "lettuce", "spinach", "fresh" },
            ["Bakery"] = new[] { "bread", "loaf", "bakery", "bagel", "croissant", "bun" },
            ["Beverages"] = new[] { "soda", "cola", "juice", "drink", "beverage", "coffee", "tea", "water" },
            ["Snacks"] = new[] { "chips", "snack", "cracker", "candy", "chocolate", "pretzel" },
            ["Frozen"] = new[] { "frozen", "ice", "ice cream", "freezer", "frozen pizza", "peas" },
            ["Household"] = new[] { "paper towel", "cleaning", "detergent", "household", "trash bag", "towel" }
        };

        private static ITransformer? _featureModel;
        private static ITransformer? _classifierModel;
        private static PredictionEngine<TrainData, TrainPrediction>? _predictor;
        private static Dictionary<string, float[]>? _categoryEmbeddings;
        private static readonly object _modelLock = new();
        private static readonly object _predictLock = new();

        private class InputData { public string InputText { get; set; } = ""; }
        private class EmbeddingOutput { public float[] Features { get; set; } = Array.Empty<float>(); }

        private static void EnsureModel()
        {
            if (_predictor != null && _categoryEmbeddings != null) return;

            lock (_modelLock)
            {
                if (_predictor != null && _categoryEmbeddings != null) return;

                var trainingData = new List<TrainData>
                {
                    // Dairy
                    new TrainData { Text = "whole milk milk carton milk gallon cheese", Label = "Dairy" },
                    new TrainData { Text = "cheddar cheese cheese block cream milk", Label = "Dairy" },
                    // Produce
                    new TrainData { Text = "bananas apple orange fresh fruit produce", Label = "Produce" },
                    new TrainData { Text = "lettuce spinach salad vegetable produce", Label = "Produce" },
                    // Bakery
                    new TrainData { Text = "sourdough bread loaf bakery croissant", Label = "Bakery" },
                    new TrainData { Text = "bagel bread roll bakery", Label = "Bakery" },
                    // Beverages
                    new TrainData { Text = "cola soda soda pack beverage juice", Label = "Beverages" },
                    new TrainData { Text = "coffee tea drink beverage", Label = "Beverages" },
                    // Snacks
                    new TrainData { Text = "potato chips snack crisps", Label = "Snacks" },
                    new TrainData { Text = "chocolate candy snack bar", Label = "Snacks" },
                    // Frozen
                    new TrainData { Text = "frozen pizza frozen peas ice cream", Label = "Frozen" },
                    // Household
                    new TrainData { Text = "paper towels cleaning household detergent", Label = "Household" },
                };

                // Include a text featurizer for the product names and category labels
                var embeddingCorpus = trainingData
                    .Select(td => new InputData { InputText = td.Text })
                    .Concat(Categories.Select(cat => new InputData { InputText = cat }))
                    .ToArray();

                var embeddingPipeline = ml.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: "InputText");
                var embeddingData = ml.Data.LoadFromEnumerable(embeddingCorpus);
                _featureModel = embeddingPipeline.Fit(embeddingData);

                var trainDataView = ml.Data.LoadFromEnumerable(trainingData);
                var trainPipeline = ml.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(TrainData.Text))
                    .Append(ml.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: nameof(TrainData.Label)))
                    .Append(ml.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "Features"))
                    .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel", "LabelKey"));

                _classifierModel = trainPipeline.Fit(trainDataView);
                _predictor = ml.Model.CreatePredictionEngine<TrainData, TrainPrediction>(_classifierModel);

                // Precompute embeddings for category labels so the cosine fallback is fast and stable
                var categoryDataView = ml.Data.LoadFromEnumerable(Categories.Select(cat => new InputData { InputText = cat }));
                var categoryTransformed = _featureModel.Transform(categoryDataView);
                var categoryVectors = ml.Data.CreateEnumerable<EmbeddingOutput>(categoryTransformed, reuseRowObject: false).ToArray();
                _categoryEmbeddings = Categories
                    .Select((cat, idx) => (cat, idx))
                    .ToDictionary(pair => pair.cat, pair => categoryVectors[pair.idx].Features);
            }
        }

        private class TrainData { public string Text { get; set; } = ""; public string Label { get; set; } = ""; }
        private class TrainPrediction
        {
            [ColumnName("PredictedLabel")]
            public string Category { get; set; } = "";
        }

        private static float[] Embed(string text)
        {
            EnsureModel();
            var data = ml.Data.LoadFromEnumerable(new[] { new InputData { InputText = text ?? string.Empty } });
            var transformed = _featureModel!.Transform(data);
            return ml.Data.CreateEnumerable<EmbeddingOutput>(transformed, reuseRowObject: false).First().Features;
        }

        private static float CosineSimilarity(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length == 0 || b.Length == 0) return 0f;
            var dot = 0.0f;
            for (int i = 0; i < Math.Min(a.Length, b.Length); i++) dot += a[i] * b[i];
            var magA = (float)Math.Sqrt(a.Sum(x => x * x));
            var magB = (float)Math.Sqrt(b.Sum(x => x * x));
            if (magA == 0f || magB == 0f) return 0f;
            return dot / (magA * magB);
        }

        public static string Predict(string productName, string description)
        {
            try
            {
                var text = $"{productName ?? string.Empty} {description ?? string.Empty}".Trim();
                if (string.IsNullOrWhiteSpace(text))
                    return "General";

                var keywordCategory = ScoreKeywords(text);
                if (!string.IsNullOrEmpty(keywordCategory))
                    return keywordCategory;

                EnsureModel();

                if (_predictor != null)
                {
                    TrainPrediction prediction;
                    lock (_predictLock)
                    {
                        prediction = _predictor.Predict(new TrainData { Text = text });
                    }

                    if (!string.IsNullOrWhiteSpace(prediction?.Category))
                        return prediction.Category;
                }

                // Fallback to similarity-based approach
                var productVec = Embed(text);
                if (_categoryEmbeddings == null || _categoryEmbeddings.Count == 0)
                    return "General";

                return _categoryEmbeddings
                    .OrderByDescending(kv => CosineSimilarity(productVec, kv.Value))
                    .FirstOrDefault().Key ?? "General";
            }
            catch
            {
                // If anything goes wrong, return a safe default
                return "General";
            }
        }

        private static string? ScoreKeywords(string text)
        {
            var lower = text.ToLowerInvariant();
            string? bestCategory = null;
            var bestScore = 0;

            foreach (var kvp in KeywordSignals)
            {
                var score = 0;
                foreach (var signal in kvp.Value)
                {
                    if (string.IsNullOrWhiteSpace(signal)) continue;
                    if (lower.Contains(signal))
                        score++;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestCategory = kvp.Key;
                }
            }

            return bestScore > 0 ? bestCategory : null;
        }
    }
}
