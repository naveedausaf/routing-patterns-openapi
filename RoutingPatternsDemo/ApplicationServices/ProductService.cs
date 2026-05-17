using System.Collections.Concurrent;
using RoutingPatternsDemo.Domain;

namespace RoutingPatternsDemo.ApplicationServices;

public class ProductService : IProductService
{
    private static readonly ConcurrentDictionary<Guid, Product> _products = new();

    public Task<Product?> GetProductById(Guid id)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
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
        _products[product.Id] = product;
        return Task.FromResult(product.Id);
    }
}