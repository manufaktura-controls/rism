using Manufaktura.RismCatalogue.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Xml;

namespace Manufaktura.RismCatalogue.Migration
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var path = @"C:\Databases\rismAllMARCXMLexample\rism_130616_example.xml";
            using (var db = new RismDbContext(new DbContextOptionsBuilder().UseMySql("server=localhost;database=manufaktura-rism;uid=admin;pwd=123123").Options))
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = XmlReader.Create(fs))
                    {
                        while (reader.Read())
                        {
                           
                        }
                    }
                }
            }
        }
    }
}