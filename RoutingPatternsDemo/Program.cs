using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RoutingPatternsDemo.ApplicationServices;
using RoutingPatternsDemo.Handlers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// adds and registers the OpenAPI document generator and related services in the DI container
builder.Services.AddOpenApi();

//Register application services that would be injected into handlers
//(separation of business logic from endpoint handlers, as per Clean Architecture)
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// PATTERN 1: Map application endpoints by calling the static route mapping method MapRoutes or MapRoutesAndDescribe on every Handlers class in teh project.

// First, create the top level route group, for `/` as done below,
// or for `/v1` if you are implementing versioning by route prefix.
var topLevelRouteGroup = app.MapGroup("/v1");

// Second, call `MapRoutes` or `MapRoutesAndDescribe` on every Handlers class in the project, passing the top level route group as an argument.
// Each Handlers class would register routes for its handlers relative to this top level route group.
ProductHandlers.MapRoutesAndDescribe(topLevelRouteGroup);

// END OF PATTERN 1 IN THIS FILE (more in ProductHandlers.cs)

// adds the middleware that would autogenerate and serve the OpenAPI schema for this API at `/openapi/v1.json`
// See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-10.0&tabs=visual-studio%2Cvisual-studio-code
app.MapOpenApi();

// This adds the `/scalar/v1` endpoint and renders a brilliant UI at it based on the OpenAPI spec that is served at `/openapi/v1.json`. 
//
// This UI allows the spec to be browsed interactively at similarly to Swagger UI and even tested against the running API.
//
// See: https://blog.scalar.com/p/how-net-9-and-scalar-solve-the-problem
app.MapScalarApiReference();


app.Run();
