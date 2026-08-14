namespace Zenvera.AiExamples.Applied.Infrastructure.Persistence;

public static class CatalogSeed
{
    public static async Task EnsureSeededAsync(CatalogDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        context.Products.AddRange(
            new Product { Name = "Solar Flashlight", Description = "Charges in daylight and runs for eight hours.", Price = 19.99m },
            new Product { Name = "Hiking Poles", Description = "Adjustable aluminium poles for steep descents.", Price = 24.99m },
            new Product { Name = "Rain Jacket", Description = "Waterproof shell with taped seams and a storm hood.", Price = 49.99m },
            new Product { Name = "Four Season Sleeping Bag", Description = "Rated to minus ten degrees with down baffles.", Price = 129.99m },
            new Product { Name = "Two Person Tent", Description = "Freestanding shelter that pitches in under five minutes.", Price = 179.99m },
            new Product { Name = "Camping Stove", Description = "Boils a litre in three minutes on standard gas canisters.", Price = 39.99m },
            new Product { Name = "Insulated Bottle", Description = "Keeps drinks cold all day or hot overnight.", Price = 29.99m },
            new Product { Name = "Trail Running Shoes", Description = "Aggressive tread for gravel and wet rock.", Price = 89.99m },
            new Product { Name = "Daypack", Description = "Twenty litre pack with a padded back panel.", Price = 59.99m },
            new Product { Name = "Merino Base Layer", Description = "Regulates temperature and resists odour.", Price = 44.99m },
            new Product { Name = "Head Torch", Description = "Hands-free lighting with a red night mode.", Price = 34.99m },
            new Product { Name = "Portable Power Bank", Description = "Twenty thousand milliamp hours for multi-day trips.", Price = 54.99m });

        await context.SaveChangesAsync(cancellationToken);
    }
}
