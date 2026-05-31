using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace RoutingPatternsDemo.UnitTests;

public class HandlerTestApplicationFactory
    : WebApplicationFactory<Program>
{

    protected override IHost CreateHost(IHostBuilder builder)
    {
        //declare the environment as "UnitTesting"
        //as this allows the startup code in Program.cs
        //to modify its behaviour
        builder.UseEnvironment(ConfigConsts.UnitTestingEnvironmentName);

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ConfigureTestServices);
    }

    // Subclasses override this instead of providing a lambda
    protected virtual void ConfigureTestServices(IServiceCollection services) { }

}