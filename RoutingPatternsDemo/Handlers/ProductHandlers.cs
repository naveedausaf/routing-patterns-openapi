using Microsoft.AspNetCore.Http.HttpResults;
using RoutingPatternsDemo.ApplicationServices;
using RoutingPatternsDemo.Domain;

namespace RoutingPatternsDemo.Handlers;

public static class ProductHandlers
{
    // PATTERN 1: 

    // This nested class contains names of handlers in this Handlers class as string constants. These names are attached to handlers when registering routes using `.WithName` and again used for generating generating links to these handlers in other handlers such as HandleCreateProduct which creates a Product then returns a GET route for the created product in its response.
    public static class HandlerNames
    {
        public const string GetProduct = "get-product";
        public const string CreateProduct = "create-product";

    }

    public const string RoutePrefix = "/products";



    // The MapRoutesAndDescribe static method registers all the handlers contained in this Handlers class relative to the baseRouteGroup passed as an argument and under that, within a route segment - `/products` in this case - that denotes this group of handlers.
    //          
    // It also registers OpenAPI metadata like operation names, summary and tags for these handlers.
    // Open API metadata is also collected automatically from handler method signatures, attributes decalred on the handler and its parameters, and, starting in .NET 10, from XML documentation comments.
    // See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/include-metadata?view=aspnetcore-10.0&tabs=minimal-apis
    internal static RouteGroupBuilder MapRoutesAndDescribe(RouteGroupBuilder baseRouteGroup)
    {
        // Open API Tag "Product Operations" is added to the group of handlers in this Handlers class under Route prefix `/products`
        var routeBuilder = baseRouteGroup.MapGroup(RoutePrefix).WithTags("Product Operations");

        routeBuilder.MapPost("/", HandleCreateProduct).WithName(HandlerNames.CreateProduct).WithSummary("Creates a new product in the system.");

        // PATTERN 3:
        // Not decalring route constraints like {id:guid} in the route template, and instead relying on model binding and OpenAPI metadata to validate the incoming request and return a 400 Bad Request if the id is not a valid guid.
        routeBuilder.MapGet("/{id}", HandleGetProduct).WithName(HandlerNames.GetProduct).WithSummary("Fetches the product details for a given product id.");
        // END OF PATTERN 3

        return routeBuilder;

    }
    // END OF PATTERN 1 IN THIS FILE (more in Program.cs)


    /// <summary>
    /// Fetches the product details for a given product id.
    /// </summary>
    /// <param name="id">Id of the product to fetch from the system.</param>
    internal static async Task<Results<Ok<Product>, NotFound>> HandleGetProduct(Guid id, IProductService productService)
    {

        var result = await productService.GetProductById(id);
        if (result == null)
        {

            return TypedResults.NotFound();

        }

        return TypedResults.Ok(result);

    }

    /// <summary>
    /// Creates a new product in the database.
    /// </summary>
    /// <param name="p">Details of the product to be created.</param>
    /// <response code="201">Product created successfully. URL to GET the newly created product is in the <c>Location</c> response header.</response>
    internal static async Task<Created> HandleCreateProduct(CreateProductArgs p, IProductService productService, LinkGenerator linkGen)
    {

        // PATTERN 2:
        // This handler returns a 201 Created response with the URL to the newly created resource in the Location header.
        // So as per PATTERN 2, it takes in a LinkGenerator as an argument. This is resolved from Dependency Injection container when the handler is invoked.
        // This is used by the handler to generate the URL to the handler that beused to GET the newly created resource. 
        // As per the pattern, the handler is referenced using the string constant for its name in the nested HandlerNames static class, using which it was also registered in MapRoutesAndDescribe.
        var result = await productService.CreateProduct(p);


        var res = TypedResults.Created(linkGen.GetPathByName(HandlerNames.GetProduct, new { id = result }));

        return res;
        // END OF PATTERN 2

    }
}
