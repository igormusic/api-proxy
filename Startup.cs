
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using ProxyKit;

namespace api_proxy
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddProxy();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/api", api =>
            {
                api.RunProxy(async context =>
                {
                    var forwardContext = context
                        .ForwardTo("http://localhost:8080")
                        .CopyXForwardedHeaders();
                        
                        var byteArray = Encoding.ASCII.GetBytes("admin:password");
                        forwardContext.UpstreamRequest.Headers.Authorization = 
                            new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(byteArray));

                    return await forwardContext.Send();
                });
            });
/* 
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            }); */
        }
    }
}
