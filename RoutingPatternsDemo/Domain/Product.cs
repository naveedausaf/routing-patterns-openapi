namespace RoutingPatternsDemo.Domain;

public class Product
{
    /// <summary>
    /// id of the product
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Name of the product
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Description of the product
    /// </summary>
    public required string Description { get; set; }
    /// <summary>
    /// Image URL of the product
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Price per unit of the product
    /// </summary>
    public required decimal Price { get; set; }


}