using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Proxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            services.AddControllers();
            services.AddProxies();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            var options = ProxyOptions.Instance
                .WithShouldAddForwardedHeaders(false)
                .WithHttpClientName("MyCustomClient")
                .WithIntercept(context =>
                {
                    if (context.Request.Method == "OPTIONS")
                    {
                        context.Response.StatusCode = 204;
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        context.Response.Headers.Add("Access-Control-Allow-Headers", "X-Requested-With, user-agent");
                        context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                        //await context.Response.WriteAsync("I don't like this port, so I am not proxying this request!");
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                });
                //.WithBeforeSend((c, hrm) =>
                //{
                //            // Set something that is needed for the downstream endpoint.
                //            hrm.Headers.Authorization = new AuthenticationHeaderValue("Basic");

                //    return Task.CompletedTask;
                //})
                //.WithAfterReceive((c, hrm) =>
                //{
                //    // Alter the content in  some way before sending back to client.
                //    //var newContent = new StringContent("It's all greek...er, Latin...to me!");
                //    //hrm.Content = newContent;
                //    hrm.Headers.Add("Access-Control-Allow-Origin", "*");
                //    hrm.Headers.Add("Access-Control-Allow-Headers", "X-Requested-With");
                //    hrm.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");

                //    return Task.CompletedTask;
                //});
                //.WithHandleFailure(async (c, e) =>
                //{
                //            // Return a custom error response.
                //            c.Response.StatusCode = 403;
                //    await c.Response.WriteAsync("Things borked.");
                //});

            app.UseProxy("{*arg1}", async (args) =>
            {
                string url = "http://localhost:8888/" + args["arg1"];
                return await Task.FromResult<string>(url);
            }, options);

            //app.UseRouting();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
        }
    }
}
