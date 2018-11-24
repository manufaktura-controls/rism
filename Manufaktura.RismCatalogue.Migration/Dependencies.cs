using Manufaktura.RismCatalogue.Migration.Services;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Ninject;
using Ninject.Modules;
using System;

namespace Manufaktura.RismCatalogue.Migration
{
    public class Dependencies : NinjectModule
    {
        private static Lazy<StandardKernel> kernel = new Lazy<StandardKernel>(() =>
        {
            var k = new StandardKernel();
            k.Load(typeof(Dependencies).Assembly);
            return k;
        });

        public static StandardKernel Instance => kernel.Value;

        public static RismDbContext CreateContext()
        {
            var context = new RismDbContext(new DbContextOptionsBuilder().UseMySql("server=localhost;database=manufaktura-rism;uid=admin;pwd=123123").Options);
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
        }

        public override void Load()
        {
            Bind<RismDbContext>().ToMethod(a => CreateContext());
            Bind<MigrationService>().ToSelf().InSingletonScope();
            Bind<PlaineAndEasieService>().ToSelf().InSingletonScope();
            //Bind<HashGenerationService>().ToSelf().InSingletonScope();
            Bind<HashGenerationServiceForDistinctIncipits>().ToSelf().InSingletonScope();
        }
    }
}