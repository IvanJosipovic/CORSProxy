using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Proxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CORSProxy
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
            services.AddControllers();

            var options = ProxyOptions.Instance
                .WithShouldAddForwardedHeaders(false)
                .WithHttpClientName("ProxyClient")
                .WithIntercept(context =>
                {
                    if (context.Request.Method == "OPTIONS")
                    {
                        context.Response.StatusCode = 204;

                        var headers = new List<string>
                        {
                            "Access-Control-Allow-Origin",
                            "Access-Control-Allow-Methods",
                            "Access-Control-Allow-Headers",
                            "Access-Control-Allow-Max-Age"
                        };

                        foreach (var header in headers)
                        {
                            context.Response.Headers[header] = Configuration.GetValue<string>(header);
                        }

                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                }).WithAfterReceive((context, response) =>
                {
                    const string header = "Access-Control-Allow-Origin";
                    
                    if (response.Headers.Contains(header))
                    {
                        response.Headers.Remove(header);
                    }
                    
                    response.Headers.Add(header, Configuration.GetValue<string>(header));

                    return Task.CompletedTask;
                });

            services.AddSingleton(options);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
