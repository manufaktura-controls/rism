using Manufaktura.RismCatalogue.Model;
using Microsoft.EntityFrameworkCore;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

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

        public override void Load()
        {
            Bind<RismDbContext>().ToMethod(a => new RismDbContext(new DbContextOptionsBuilder().UseMySql("server=localhost;database=manufaktura-rism;uid=admin;pwd=123123").Options));
        }
    }
}
