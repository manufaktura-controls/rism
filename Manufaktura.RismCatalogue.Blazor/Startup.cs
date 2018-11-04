using Manufaktura.RismCatalogue.Shared.Services;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Manufaktura.RismCatalogue.Blazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ServerSideSettingsService>();
            services.AddSingleton<ScoreRendererService>();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("blazorHost");
        }
    }
}
