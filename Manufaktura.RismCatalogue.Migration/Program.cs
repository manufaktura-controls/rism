using Manufaktura.RismCatalogue.Migration.Services;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Services;
using Ninject;
using System;
using System.Diagnostics;

namespace Manufaktura.RismCatalogue.Migration
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var db = Dependencies.Instance.Get<RismDbContext>();
            db.Database.EnsureCreated();

            Dependencies.Instance.Get<MigrationService>().Migrate(5000);
            Dependencies.Instance.Get<LSHService>().GenerateHashes(10, 15);

            sw.Stop();
            Console.WriteLine($"Finished in {sw.Elapsed}.");
            Console.ReadLine();
        }
    }
}