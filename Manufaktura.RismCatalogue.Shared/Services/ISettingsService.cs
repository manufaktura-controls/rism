using Manufaktura.Controls.Rendering.Implementations;
using System.Threading.Tasks;

namespace Manufaktura.RismCatalogue.Shared.Services
{
    public interface ISettingsService
    {
        Task InitializeAsync();

        HtmlScoreRendererSettings GetRendererSettings();

        bool IsInitialized { get; }
    }
}