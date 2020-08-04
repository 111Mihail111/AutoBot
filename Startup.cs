using AutoBot.Area.API;
using AutoBot.Area.Collecting—ryptocurrencies;
using AutoBot.Area.Collecting—ryptocurrencies.Cranes;
using AutoBot.Area.Collecting—ryptocurrencies.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoBot
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddScoped<IFreeBitcoin, FreeBitcoin>();
            services.AddScoped<IRuCaptcha, RuCaptcha>();
            services.AddScoped<IMoonBitcoin, MoonBitcoin>();
            services.AddScoped<IBonusBitcoin, BonusBitcoin>();
            services.AddScoped<IMoonDogecoin, MoonDogecoin>();
            services.AddScoped<IRuCaptchaController, RuCaptchaController>();
            services.AddScoped<IMoonLitecoin, MoonLitecoin>();
            services.AddScoped<IMoonDash, MoonDash>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Start}/{action=Index}/{string?}");
            });
        }
    }
}
