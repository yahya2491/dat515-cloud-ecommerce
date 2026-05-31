// Simple text similarity function using keyword overlap
function keywordSimilarity(a, b) {
  const wordsA = new Set(a.toLowerCase().split(/\W+/));
  const wordsB = new Set(b.toLowerCase().split(/\W+/));
  const intersection = new Set([...wordsA].filter((word) => wordsB.has(word)));
  return intersection.size / Math.max(wordsA.size, wordsB.size);
}

/**
 * Recommend up to 3 products similar to a given one
 * based on category + name/description similarity.
 */
export function recommendProducts(purchasedProduct, allProducts) {
  if (!purchasedProduct || !allProducts?.length) return [];

  const recommendations = allProducts
    .filter((p) => p.id !== purchasedProduct.id)
    .map((p) => {
      let score = 0;

      // Boost if same category
      if (p.category === purchasedProduct.category) score += 1.0;

      // Add similarity from name/description overlap
      score += keywordSimilarity(p.name, purchasedProduct.name) * 0.5;
      score += keywordSimilarity(p.description || "", purchasedProduct.description || "") * 0.5;

      return { ...p, score };
    })
    .sort((a, b) => b.score - a.score)
    .slice(0, 3); // top 3

  return recommendations;
}
