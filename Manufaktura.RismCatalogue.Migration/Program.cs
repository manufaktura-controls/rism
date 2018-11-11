using Manufaktura.RismCatalogue.Migration.Services;
using Ninject;

namespace Manufaktura.RismCatalogue.Migration
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Dependencies.Instance.Get<MigrationService>().Migrate();
        }
    }
}