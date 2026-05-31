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

// Configure ForwardedHeadersMiddleware below in Development environment only so that Scalar UI can work properly in GitHub Codespace when the API is launched using a debug launch configuration.
// For detailed reasoning, see comment on `app.UseForwardedHeaders()` below in the middleware pipeline configuration.
if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All;
        // Clear restrictions so that the proxy headers from GitHub Codespaces are accepted.
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });
}

var app = builder.Build();


// In a GitHub Codespace, when the API is launched using a debug launch config (such as as the one in launch.json in this workspace, launched using F5), it would launch at the configured port e.g. http://localhost:3022 in dev container running in GitHub Codespace.
// 
// This forwarded port would be accessible at a Codespace-generated https URL because of the Port Forwarding feature of GitHub Codespaces. This is implemented using a reverse proxy (the "Codespace proxy") that forwards requests from the Codespace-generated URL to the port on which the API is running in the Codespace. The Codespace proxy adds standard proxy headers to the forwarded request, including `X-Forwarded-For`, `X-Forwarded-Proto`, and `X-Forwarded-Host`.
//
// When the user launches the app using a VS Code launch config in their CodeSpace, this would launch the Scalar UI automatically but this would be opened by the port forwarding feature with the codespace generated URL as that maps to `https://localhost:3022`. 
//
// When the request for this URL - https://<codespace gnerated domani name>/scalar` - comes in to the app running on http://localhost:3022 in dev container, Scalar UI sees the original URL because port forwarding is enabled. It embeds it in the Scalar UI that it serves and so sending tests the API from the user's browser would use this URL rather than http://localhost:3022. This way testing feature in Scalar UI works.
//
// We only want to enable it explicitly as done below in Development (this is the envrionment that API launched in debug launch config defined in launch.json uses). In any other environment it would be registered automatically if `ASPNETCORE_FORWARDEDHEADERS_ENABLED` environment variable is set; this can be set at deployment if needed.
if (app.Environment.IsDevelopment())
{
    // Must be very early in the pipeline so it changes the Request object for everything that follows
    app.UseForwardedHeaders();

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

if (app.Environment.IsDevelopment())
{
    // In a browser-based GitHub Codespace, even if you set "serverReadyAction" attribute of a debug launch configuration in launch.json to `/scalar` (as done in the default launch config that is launched by pressing F5), when you launch the launch config, it may still be the root (`/`) that gets opened.
    // However, it is very convenient when launching the API in a launch config, for Scalar UI to open so we can test and debug the API.
    // Therefore we map `/` to `/scalar` in Development.
    app.MapGet("/", () => Results.Redirect("/scalar")).
    ExcludeFromDescription();
}


app.Run();
