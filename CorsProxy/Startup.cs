using System.Threading.Tasks;
using AspNetCore.Proxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CorsProxy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddProxies();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var options = ProxyOptions.Instance
                .WithShouldAddForwardedHeaders(false)
                .WithHttpClientName("ProxyClient")
                .WithIntercept(context =>
                {
                    if (context.Request.Method == "OPTIONS")
                    {
                        context.Response.StatusCode = 204;
                        
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                        context.Response.Headers.Add("Access-Control-Allow-Methods", "*");
                        
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                });

            app.UseProxy("{*arg1}", async (args) =>
            {
                string url = "http://localhost:8888/" + args["arg1"];
                return await Task.FromResult<string>(url);
            }, options);
        }
    }
}
