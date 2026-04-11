using RoutingPatternsDemo.Domain;

namespace RoutingPatternsDemo.ApplicationServices;

public interface IProductService
{
    Task<Product?> GetProductById(Guid id);
    Task<Guid> CreateProduct(CreateProductArgs createProductArgs);

}