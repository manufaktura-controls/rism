using Manufaktura.RismCatalogue.Migration.Services;
using Manufaktura.RismCatalogue.Model;
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

            Dependencies.Instance.Get<MigrationService>().Migrate(100000);
            Dependencies.Instance.Get<HashGenerationService>().GenerateHashes(3, 15);

            sw.Stop();
            Console.WriteLine($"Finished in {sw.Elapsed}.");
            Console.ReadLine();
        }
    }
}