using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RoutingPatternsDemo;
using RoutingPatternsDemo.ApplicationServices;
using RoutingPatternsDemo.Handlers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Config key value pairs are not available in two scenarios:
//
// 1. When the API is being run during build to generate OpenAPI document. We detect this by testing that the calling assembly name is "GetDocument.Insider" as per Microsoft documentation.
// See: https://www.naveedausaf.com/accept-the-official-hack-build-time-openapi-detection-in-net-10-minimal-apis
//
// 2. When the API is being run in the special "UnitTesting" environment. In this environment. This environment name is set (in a custom WebApplicationFactory-derived class) when running certain tests that require the request processing pipeline to be set up (i.e. for Program.cs to run). 
//
// In this project there is only one such test. This fetches OpenAPI document from `/openapi/v1.json`. This does not require any services other than `.AddOpenApi()` which IS registered in "UnitTesting" environment as it does not need any config data.
//
// Hence both scenarios can unfold successfully when `isNoConfigAvailable` is true. 

// We use this variable later in Program.cs to conditionally exclude any service or middleware from registration that relies on config key/value pairs being available during startup (i.e. when Program.cs runs) e.g.:
//
// * DbContext (because a AddDbContext method in Program.cs might need config to get the connection string to the database)
// * CORS (because config tells us the allowed origins. We need to pass these to CORS setup calls)
// * Health Checks (because these requires DbContext)
// * OpenTelemetry (I don't think it would fail if there is not config - Otel clients are designed that way in any language - but would exclude it out of caution)
//
var isNoConfigAvailable = (Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider") || builder.Environment.IsEnvironment(ConfigConsts.UnitTestingEnvironmentName);

// The snippet below disables DI graph validation on builder.Build() so errors are not thrown if a dependency that is injected into a service that is registered in DI has not been registered.
//
// See Andrew Lock's post for explanation of what ValidateOnBuild does:
// https://andrewlock.net/new-in-asp-net-core-3-service-provider-validation/
//
// You might want to conditionally exclude certain services from initialization and adding to DI when isNoConfigAvailable, as explained in comments on `isNoConfigAvailable` above.
//
// When isNoConfigAvailable == true and any services are conditionally excluded, then there may be an error in Program.cs when on builder.Build() if one of the registered services has a DI-injected dependency that was not registered.
//
// This snippet stops such errors from being thrown by setting ValidateOnBuild to false in that situation. This allows Program.cs execution to proceed past builder.Build() and complete successfully. Then:
//
// 1. If isNoConfigAvailable because the API is being run during build to generate OpenAPI document, then the only endpoint that would be called is `/openapi/v1.json` and this does not ned any of the services we register here except .AddOpenApi(). This service is always added (whether or not isNoConfigAvailable)
//
// 2. If environment is "UnitTesting" then the OpenAPIDocument test only request /openapi/v1/json to fetch the document from the API and this endpoint doesn't requrie any extra services of config to run so would run fine.
//
// So both uses cases of execution if isNoConfigAvailable would still succeed.
if (isNoConfigAvailable)
{
    builder.Host.UseDefaultServiceProvider(options => options.ValidateOnBuild = false);
}

if (isNoConfigAvailable)
{
    // You would use conditional blocks like this to exclude any services registrations, e.g. CORS services and .AddDbContext() calls that require config values to be available during startup.
}

// adds and registers the OpenAPI document generator and related services in the DI container
builder.Services.AddOpenApi();

//Register application services that would be injected into handlers
//(separation of business logic from endpoint handlers, as per Clean Architecture).
//
// These don't directly need config 
// (although they use DbContexts that do require conencton string in config; but DbContexts will not be registered. This is where setting `options.ValidateOnBuild = false` prevents an error being thrown on `builder.Build()` )
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (isNoConfigAvailable)
{
    // You would use conditional blocks like this to exclude any middleware registrations, e.g. CORS middleware that require config values to be available and therefore cannot startup.
}

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
