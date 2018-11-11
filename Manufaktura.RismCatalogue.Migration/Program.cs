using Manufaktura.RismCatalogue.Migration.Services;
using Manufaktura.RismCatalogue.Shared.Services;
using Ninject;

namespace Manufaktura.RismCatalogue.Migration
{
    internal class Program
    {
        private static void Main(string[] args)
        {
           // Dependencies.Instance.Get<MigrationService>().Migrate();
            Dependencies.Instance.Get<LSHService>().GenerateHashes(10, 10);
        }
    }
}