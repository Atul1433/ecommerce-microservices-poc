using Marten.Schema;

namespace Catalog.API.Data;

public class CatalogInitialData : IInitialData
{
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        using var session = store.LightweightSession();

        if (await session.Query<Product>().AnyAsync())
            return;

        // Store categories and brands first
        var categories = GetPreconfiguredCategories();
        var brands = GetPreconfiguredBrands();

        if (!await session.Query<Category>().AnyAsync())
            session.Store<Category>(categories);
        if (!await session.Query<Brand>().AnyAsync())
            session.Store<Brand>(brands);
        await session.SaveChangesAsync();

        // Marten UPSERT will cater for existing records
        var products = GetPreconfiguredProducts(categories, brands);
        session.Store<Product>(products);
        await session.SaveChangesAsync();
    }

    private static IEnumerable<Category> GetPreconfiguredCategories() => new List<Category>()
    {
        new Category()
        {
            Id = Guid.NewGuid(),
            Name = "Smartphones",
            Slug = "smartphones",
            Description = "Latest smartphones and mobile devices",
            Icon = "📱",
            IsActive = true,
            DisplayOrder = 1
        },
        new Category()
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            Slug = "electronics",
            Description = "Electronic gadgets and devices",
            Icon = "⚡",
            IsActive = true,
            DisplayOrder = 2
        },
        new Category()
        {
            Id = Guid.NewGuid(),
            Name = "Cameras",
            Slug = "cameras",
            Description = "Professional and consumer cameras",
            Icon = "📷",
            IsActive = true,
            DisplayOrder = 3
        },
        new Category()
        {
            Id = Guid.NewGuid(),
            Name = "Home & Kitchen",
            Slug = "home-kitchen",
            Description = "Home appliances and kitchen equipment",
            Icon = "🏠",
            IsActive = true,
            DisplayOrder = 4
        }
    };

    private static IEnumerable<Brand> GetPreconfiguredBrands() => new List<Brand>()
    {
        new Brand()
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            Logo = "apple-logo.png",
            Description = "Technology company known for iPhones, MacBooks, and more",
            Country = "United States",
            Website = "www.apple.com"
        },
        new Brand()
        {
            Id = Guid.NewGuid(),
            Name = "Samsung",
            Logo = "samsung-logo.png",
            Description = "Leading electronics and smartphone manufacturer",
            Country = "South Korea",
            Website = "www.samsung.com"
        },
        new Brand()
        {
            Id = Guid.NewGuid(),
            Name = "Xiaomi",
            Logo = "xiaomi-logo.png",
            Description = "Chinese electronics company offering affordable smartphones",
            Country = "China",
            Website = "www.xiaomi.com"
        },
        new Brand()
        {
            Id = Guid.NewGuid(),
            Name = "HTC",
            Logo = "htc-logo.png",
            Description = "Taiwanese smartphone and electronics manufacturer",
            Country = "Taiwan",
            Website = "www.htc.com"
        },
        new Brand()
        {
            Id = Guid.NewGuid(),
            Name = "Panasonic",
            Logo = "panasonic-logo.png",
            Description = "Japanese electronics corporatio specializing in cameras and appliances",
            Country = "Japan",
            Website = "www.panasonic.com"
        }
    };

    private static IEnumerable<Product> GetPreconfiguredProducts(IEnumerable<Category> categories, IEnumerable<Brand> brands) => new List<Product>()
    {
        new Product()
        {
            Id = Guid.NewGuid(),
            Name = "iPhone 15 Pro Max",
            SKU = "IPHONE-15PM-BLK",
            CategoryId = categories.First(c => c.Slug == "smartphones").Id,
            BrandId = brands.First(b => b.Name == "Apple").Id,
            Description = "Advanced smartphone with A17 Pro chip and ProRaw camera support",
            LongDescription = "The iPhone 15 Pro Max features a stunning 6.7-inch Super Retina XDR display, titanium design, and advanced computational photography capabilities. With the powerful A17 Pro chip and professional-grade camera system, it's the ultimate device for creators and professionals.",
            ImageFile = "product-1.png",
            AdditionalImages = new List<string> { "product-1-alt1.png", "product-1-alt2.png" },
            Price = 1199.00M,
            CostPrice = 850.00M,
            DiscountPercentage = 5,
            StockQuantity = 150,
            ReorderLevel = 20,
            Weight = 0.221M,
            Dimensions = "159.9 × 77.8 × 8.25 mm",
            Color = "Black Titanium",
            Material = "Titanium",
            Tags = new List<string> { "flagship", "premium", "camera" },
            Keywords = new List<string> { "iPhone", "Apple", "smartphone", "camera", "5G" },
            IsActive = true,
            IsFeatured = true,
            CreatedAt = DateTime.UtcNow,
            AverageRating = 4.8M,
            TotalReviews = 2450
        },
        new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Samsung Galaxy S24 Ultra",
            SKU = "SGAL-S24U-WHT",
            CategoryId = categories.First(c => c.Slug == "smartphones").Id,
            BrandId = brands.First(b => b.Name == "Samsung").Id,
            Description = "Flagship Android phone with stunning display and powerful performance",
            LongDescription = "The Galaxy S24 Ultra delivers a 6.8-inch Dynamic AMOLED 2X display with Vision Booster, 200MP main camera, and advanced AI features. Designed for enthusiasts who demand the best in performance and photography.",
            ImageFile = "product-2.png",
            AdditionalImages = new List<string> { "product-2-alt1.png" },
            Price = 1299.00M,
            CostPrice = 900.00M,
            DiscountPercentage = 8,
            StockQuantity = 120,
            ReorderLevel = 15,
            Weight = 0.218M,
            Dimensions = "162.8 × 79.6 × 8.8 mm",
            Color = "Titanium Gray",
            Material = "Metal and Glass",
            Tags = new List<string> { "flagship", "android", "5G" },
            Keywords = new List<string> { "Samsung", "Galaxy", "Android", "flagship", "camera" },
            IsActive = true,
            IsFeatured = true,
            CreatedAt = DateTime.UtcNow,
            AverageRating = 4.7M,
            TotalReviews = 1890
        },
        new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Xiaomi 14 Ultra",
            SKU = "XIAO-14ULT-BLK",
            CategoryId = categories.First(c => c.Slug == "smartphones").Id,
            BrandId = brands.First(b => b.Name == "Xiaomi").Id,
            Description = "Affordable flagship with excellent camera and performance",
            LongDescription = "The Xiaomi 14 Ultra combines premium features with an affordable price point. Featuring a 6.73-inch AMOLED display, 50MP camera system, and latest processor, it's perfect for users seeking value without compromise.",
            ImageFile = "product-3.png",
            AdditionalImages = new List<string> { "product-3-alt1.png", "product-3-alt2.png", "product-3-alt3.png" },
            Price = 799.00M,
            CostPrice = 500.00M,
            DiscountPercentage = 12,
            StockQuantity = 200,
            ReorderLevel = 25,
            Weight = 0.219M,
            Dimensions = "161.1 × 75.0 × 8.2 mm",
            Color = "Midnight Black",
            Material = "Glass and Metal",
            Tags = new List<string> { "budget-friendly", "flagship", "value" },
            Keywords = new List<string> { "Xiaomi", "affordable", "camera", "smartphone" },
            IsActive = true,
            IsFeatured = false,
            CreatedAt = DateTime.UtcNow,
            AverageRating = 4.5M,
            TotalReviews = 1200
        },
        new Product()
        {
            Id = Guid.NewGuid(),
            Name = "HTC U24 Pro",
            SKU = "HTC-U24PRO-SLV",
            CategoryId = categories.First(c => c.Slug == "smartphones").Id,
            BrandId = brands.First(b => b.Name == "HTC").Id,
            Description = "Mid-range smartphone with solid performance and display",
            LongDescription = "The HTC U24 Pro offers a balanced package with a 6.7-inch display, capable processor, and reliable battery life. Ideal for everyday users who want a dependable smartphone without breaking the bank.",
            ImageFile = "product-4.png",
            Price = 499.00M,
            CostPrice = 350.00M,
            DiscountPercentage = 10,
            StockQuantity = 180,
            ReorderLevel = 20,
            Weight = 0.198M,
            Dimensions = "160.5 × 74.0 × 8.6 mm",
            Color = "Silver",
            Material = "Plastic and Metal",
            Tags = new List<string> { "mid-range", "reliable", "value" },
            Keywords = new List<string> { "HTC", "smartphone", "mid-range" },
            IsActive = true,
            IsFeatured = false,
            CreatedAt = DateTime.UtcNow,
            AverageRating = 4.2M,
            TotalReviews = 650
        },
        new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Panasonic Lumix S1R",
            SKU = "PANA-S1R-BLK",
            CategoryId = categories.First(c => c.Slug == "cameras").Id,
            BrandId = brands.First(b => b.Name == "Panasonic").Id,
            Description = "Professional full-frame mirrorless camera for serious photographers",
            LongDescription = "The Lumix S1R is a high-resolution full-frame mirrorless camera with 47.3MP sensor, exceptional image quality, and weather-sealed body. Built for professional photographers and content creators who demand the highest standards.",
            ImageFile = "product-5.png",
            AdditionalImages = new List<string> { "product-5-alt1.png", "product-5-alt2.png" },
            Price = 3995.00M,
            CostPrice = 2800.00M,
            DiscountPercentage = 5,
            StockQuantity = 25,
            ReorderLevel = 5,
            Weight = 1.4M,
            Dimensions = "150.5 × 107.0 × 101.0 mm",
            Color = "Black",
            Material = "Magnesium Alloy",
            Tags = new List<string> { "professional", "mirrorless", "3D" },
            Keywords = new List<string> { "Panasonic", "Lumix", "camera", "professional", "mirrorless" },
            IsActive = true,
            IsFeatured = true,
            CreatedAt = DateTime.UtcNow,
            AverageRating = 4.9M,
            TotalReviews = 380
        },
        new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Samsung 55\" QLED 4K TV",
            SKU = "SGAL-TV55-LED",
            CategoryId = categories.First(c => c.Slug == "home-kitchen").Id,
            BrandId = brands.First(b => b.Name == "Samsung").Id,
            Description = "4K Smart TV with vibrant QLED display and advanced features",
            LongDescription = "Experience stunning picture quality with Samsung's 55-inch QLED display featuring quantum dot technology. With 4K resolution, smart apps, and gaming features, it's the perfect entertainment centerpiece for your home.",
            ImageFile = "product-6.png",
            AdditionalImages = new List<string> { "product-6-alt1.png" },
            Price = 899.00M,
            CostPrice = 600.00M,
            DiscountPercentage = 15,
            StockQuantity = 45,
            ReorderLevel = 5,
            Weight = 13.5M,
            Dimensions = "1237 × 714 × 62 mm",
            Color = "Black",
            Material = "Metal and Plastic",
            Tags = new List<string> { "TV", "4K", "smart", "entertainment" },
            Keywords = new List<string> { "Samsung", "television", "4K", "QLED", "smart" },
            IsActive = true,
            IsFeatured = true,
            CreatedAt = DateTime.UtcNow,
            AverageRating = 4.6M,
            TotalReviews = 920
        },
        new Product()
        {
            Id = Guid.NewGuid(),
            Name = "Apple MacBook Pro 16\"",
            SKU = "APP-MBP16-SLV",
            CategoryId = categories.First(c => c.Slug == "electronics").Id,
            BrandId = brands.First(b => b.Name == "Apple").Id,
            Description = "Powerful laptop for professionals with M3 Max chip",
            LongDescription = "The MacBook Pro 16-inch delivers exceptional performance with Apple's M3 Max chip, 36GB unified memory, and stunning Liquid Retina XDR display. Perfect for video editors, developers, and creative professionals.",
            ImageFile = "product-7.png",
            AdditionalImages = new List<string> { "product-7-alt1.png" },
            Price = 3499.00M,
            CostPrice = 2400.00M,
            DiscountPercentage = 0,
            StockQuantity = 30,
            ReorderLevel = 5,
            Weight = 2.15M,
            Dimensions = "358 × 248 × 16.8 mm",
            Color = "Silver",
            Material = "Aluminum and Glass",
            Tags = new List<string> { "professional", "laptop", "powerful" },
            Keywords = new List<string> { "Apple", "MacBook", "laptop", "M3", "professional" },
            IsActive = true,
            IsFeatured = true,
            CreatedAt = DateTime.UtcNow,
            AverageRating = 4.8M,
            TotalReviews = 1100
        }
    };
}
