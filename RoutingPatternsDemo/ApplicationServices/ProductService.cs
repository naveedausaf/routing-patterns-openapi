using RoutingPatternsDemo.Domain;

namespace RoutingPatternsDemo.ApplicationServices;

public class ProductService : IProductService
{
    private static readonly List<Product> _products = new();

    public Task<Product?> GetProductById(Guid id)
    {
        return Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
    }

    public Task<Guid> CreateProduct(CreateProductArgs createProductArgs)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = createProductArgs.Name,
            Description = createProductArgs.Description,
            ImageUrl = createProductArgs.ImageUrl,
            Price = createProductArgs.Price
        };
        _products.Add(product);
        return Task.FromResult(product.Id);
    }
}