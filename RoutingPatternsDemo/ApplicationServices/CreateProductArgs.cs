namespace RoutingPatternsDemo.ApplicationServices;

/// <summary>
/// Details of the product to be created
/// </summary>
public record CreateProductArgs
{

    /// <summary>
    /// Name of the product
    /// </summary>
    public required string Name { get; set; }


    /// <summary>
    /// A description for the product
    /// </summary>
    public required string Description { get; set; }


    /// <summary>
    /// URL of an image of the product
    /// </summary>
    public string? ImageUrl { get; set; }


    /// <summary>
    /// Price per-unit of the product. Must be greater than or equal to zero.
    /// </summary>
    public required decimal Price { get; set; }
}