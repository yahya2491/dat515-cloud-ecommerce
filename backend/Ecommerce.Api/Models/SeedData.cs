using Bogus;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Api.Services;

namespace Ecommerce.Api.Models
{
    public static class SeedData
    {
        public static async Task EnsureSeedDataAsync(AppDbContext db)
        {
            var defaultProducts = new List<Product>
            {
                new Product
                {
                    Name = "Whole Milk",
                    Price = 3.49m,
                    Description = "1 Gallon whole milk.",
                    Stock = 50,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Cheddar Cheese",
                    Price = 4.79m,
                    Description = "Sharp cheddar cheese block.",
                    Stock = 40,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Bananas",
                    Price = 1.29m,
                    Description = "Fresh yellow bananas.",
                    Stock = 120,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Sourdough Bread",
                    Price = 5.99m,
                    Description = "Fresh bakery sourdough loaf.",
                    Stock = 30,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Cola Soda (12-pack)",
                    Price = 7.49m,
                    Description = "Classic cola flavor.",
                    Stock = 60,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Potato Chips",
                    Price = 2.49m,
                    Description = "Crispy salted potato chips.",
                    Stock = 75,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Frozen Pizza",
                    Price = 8.99m,
                    Description = "Pepperoni frozen pizza.",
                    Stock = 45,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Paper Towels (6-pack)",
                    Price = 12.99m,
                    Description = "Strong and absorbent paper towels.",
                    Stock = 25,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Organic Eggs (12-count)",
                    Price = 5.49m,
                    Description = "Cage-free large organic eggs.",
                    Stock = 80,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Greek Yogurt",
                    Price = 4.29m,
                    Description = "Plain Greek yogurt, 32 oz tub.",
                    Stock = 55,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Orange Juice",
                    Price = 6.19m,
                    Description = "Fresh squeezed orange juice, 1.5L.",
                    Stock = 45,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Granola Cereal",
                    Price = 4.89m,
                    Description = "Honey almond granola cereal, 16 oz.",
                    Stock = 70,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Peanut Butter",
                    Price = 3.99m,
                    Description = "Creamy peanut butter, 18 oz jar.",
                    Stock = 90,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Strawberry Jam",
                    Price = 3.59m,
                    Description = "Strawberry fruit spread, 14 oz.",
                    Stock = 65,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Brown Rice",
                    Price = 2.99m,
                    Description = "Whole grain brown rice, 2 lb bag.",
                    Stock = 110,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Black Beans (Canned)",
                    Price = 1.49m,
                    Description = "Organic black beans, 15 oz can.",
                    Stock = 140,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Fresh Spinach",
                    Price = 2.79m,
                    Description = "Baby spinach greens, 10 oz bag.",
                    Stock = 85,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Carrots (2 lb)",
                    Price = 2.19m,
                    Description = "Whole carrots, 2 pound bag.",
                    Stock = 95,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Tomato Soup",
                    Price = 1.99m,
                    Description = "Creamy tomato soup, 18 oz can.",
                    Stock = 75,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Olive Oil",
                    Price = 9.49m,
                    Description = "Extra virgin olive oil, 750 ml.",
                    Stock = 50,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Sea Salt Crackers",
                    Price = 3.29m,
                    Description = "Whole wheat sea salt crackers.",
                    Stock = 60,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Dark Chocolate Bar",
                    Price = 2.79m,
                    Description = "70% cocoa dark chocolate bar.",
                    Stock = 85,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Trail Mix",
                    Price = 6.39m,
                    Description = "Fruit and nut trail mix, 24 oz.",
                    Stock = 55,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Espresso Coffee Beans",
                    Price = 11.99m,
                    Description = "Whole bean espresso roast, 1 lb.",
                    Stock = 40,
                    CreatedAt = DateTime.UtcNow
                }
            };

            var existingNames = await db.Products
                .Select(p => p.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToListAsync();

            var existingNameSet = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);
            var productsToInsert = defaultProducts
                .Where(p => !existingNameSet.Contains(p.Name))
                .ToList();

            if (productsToInsert.Any())
            {
                // Assign categories using ML, but don't let ML failures stop seeding/startup
                foreach (var product in productsToInsert)
                {
                    try
                    {
                        var predicted = CategoryPredictor.Predict(product.Name ?? string.Empty, product.Description ?? string.Empty);
                        product.Category = string.IsNullOrWhiteSpace(predicted) ? "General" : predicted;
                    }
                    catch (Exception ex)
                    {
                        // Log to console during startup; a more advanced approach would use ILogger
                        Console.WriteLine($"[SeedData] Category prediction failed for '{product.Name}': {ex.Message}. Falling back to 'General'.");
                        product.Category = "General";
                    }
                }

                await db.Products.AddRangeAsync(productsToInsert);
                Console.WriteLine($"🤖 ML assigned categories & seeded {productsToInsert.Count} products...");
            }
            else
            {
                Console.WriteLine("✅ Default products already exist, skipping seed.");
            }

            // Recompute categories for existing products if they are missing or set to 'General'
            try
            {
                var toUpdate = await db.Products.Where(p => string.IsNullOrWhiteSpace(p.Category)).ToListAsync();
                if (toUpdate.Any())
                {
                    foreach (var p in toUpdate)
                    {
                        try
                        {
                            p.Category = CategoryPredictor.Predict(p.Name ?? string.Empty, p.Description ?? string.Empty);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[SeedData] Category recompute failed for '{p.Name}': {ex.Message}");
                            p.Category = "General";
                        }
                    }
                    await db.SaveChangesAsync();
                    Console.WriteLine($"🔁 Recomputed categories for {toUpdate.Count} existing products.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeedData] Error while recomputing categories: {ex.Message}");
            }

            // Seed users if empty
            if (!await db.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new User 
                    { 
                        Id = Guid.NewGuid(),
                        Username = "alice", 
                        Nickname = "Alice Smith", 
                        PasswordHash = "$2a$11$hHTxtA2.ky6TznAanHvlROdIucge/BqJ0JfGrLi2nilZhjdScRmfm",
                        Role = "User"
                    },
                    new User 
                    { 
                        Id = Guid.NewGuid(),
                        Username = "bob", 
                        Nickname = "Bob Johnson", 
                        PasswordHash = "$2a$11$hHTxtA2.ky6TznAanHvlROdIucge/BqJ0JfGrLi2nilZhjdScRmfm",
                        Role = "User"
                    },
                    new User 
                    { 
                        Id = Guid.NewGuid(),
                        Username = "admin", 
                        Nickname = "Administrator", 
                        PasswordHash = "$2a$11$tjvDOyJpTXM0UTms4CW2U.U1Y6V.e6iV5UQJlMK0hFB81inhoYYaO",
                        Role = "Admin"
                    }
                };

                await db.Users.AddRangeAsync(users);
                Console.WriteLine($"🌱 Seeding {users.Count} users...");
            }
            else
            {
                Console.WriteLine("✅ Users already exist, skipping seed.");
            }

            var changes = await db.SaveChangesAsync();
            if (changes > 0)
            {
                Console.WriteLine($"✅ Successfully saved {changes} seed data changes to database.");
            }
        }
    }
}