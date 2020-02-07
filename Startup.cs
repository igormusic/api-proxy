using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using ProxyKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace api_proxy
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddProxy();
            services.Configure<ProxyConfig>(Configuration.GetSection("ProxyConfig"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,  IOptionsMonitor<ProxyConfig> proxyOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var proxyConfig = proxyOptions.CurrentValue;

            foreach(var route in proxyConfig.ProxyRoutes){
                app.Map( route.External , api =>
                    {
                        api.RunProxy(async context =>
                        {
                            var forwardContext = context
                                .ForwardTo(route.ForwardTo)
                                .CopyXForwardedHeaders();
                                
                                var byteArray = Encoding.ASCII.GetBytes(
                                    proxyConfig.BasicAuth.User + ":" + 
                                    proxyConfig.BasicAuth.Password);

                                forwardContext.UpstreamRequest.Headers.Authorization = 
                                    new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(byteArray));

                            return await forwardContext.Send();
                        });
                    });

            }

           
        }
    }
}
