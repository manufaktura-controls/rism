using Manufaktura.Controls.Rendering.Implementations;
using System.Threading.Tasks;

namespace Manufaktura.RismCatalogue.Shared.Services
{
    public interface ISettingsService
    {
        Task<HtmlScoreRendererSettings> GetRendererSettingsAsync();
    }
}