using AutoBot.Area.API;
using AutoBot.Area.Collecting—ryptocurrencies;
using AutoBot.Area.Collecting—ryptocurrencies.Cranes;
using AutoBot.Area.Collecting—ryptocurrencies.Interface;
using AutoBot.Area.Managers;
using AutoBot.Area.Managers.Interface;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.PerformanceTasks.InternetServices;
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
            services.AddScoped<IVLike, VLike>();
            services.AddScoped<IVkTarget, VkTarget>();
            services.AddScoped<IVkManager, VkManager>();
            services.AddScoped<IClassmatesManager, ClassmatesManager>();
            services.AddScoped<IYouTubeManager, YouTubeManager>();
            services.AddScoped<IInstagramManager, InstagramManager>();
            services.AddScoped<IYandexZenManager, YandexZenManager>();
            services.AddScoped<ITumblr, TumblrManager>();
            services.AddScoped<IReddit, RedditManager>();
            services.AddScoped<IQuora, QuoraManager>();
            services.AddScoped<ISoundCloud, SoundCloudManager>();
            services.AddScoped<IVimeoManager, VimeoManager>();
            services.AddScoped<IVkMyMarket, VkMyMarket>();
            services.AddScoped<ITikTokManager, TikTokManager>();
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
