using Manufaktura.LibraryStandards.Marc;
using Manufaktura.RismCatalogue.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Manufaktura.RismCatalogue.Migration
{
    internal class Program
    {
        private static readonly Lazy<Dictionary<string, Func<Entity>>> fieldFactories = new Lazy<Dictionary<string, Func<Entity>>>(() =>
        {
            var dict = new Dictionary<string, Func<Entity>>();
            foreach (var type in typeof(Entity).Assembly.GetTypes())
            {
                var dataFieldAttribute = type.GetCustomAttribute<MarcDatafieldAttribute>();
                if (dataFieldAttribute == null) continue;

                var factoryMethod = Expression.Lambda(Expression.New(type)).Compile() as Func<Entity>;
                dict.Add(dataFieldAttribute.Tag, factoryMethod);
            }
            return dict;
        });

        public static Dictionary<string, Func<Entity>> FieldFactories => fieldFactories.Value;

        private static void Main(string[] args)
        {
            var path = @"C:\Databases\rismAllMARCXMLexample\rism_130616_example.xml";
            using (var db = new RismDbContext(new DbContextOptionsBuilder().UseMySql("server=localhost;database=manufaktura-rism;uid=admin;pwd=123123").Options))
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = XmlReader.Create(fs, new XmlReaderSettings { IgnoreWhitespace = true }))
                    {
                        while (reader.Read() && reader.Name != "record")
                        {
                            reader.MoveToContent();
                        }
                        while (true)
                        {
                            var record = reader.ReadOuterXml();
                            if (string.IsNullOrWhiteSpace(record)) break;

                            var recordElement = XElement.Parse(record);
                            ParseRecord(recordElement, db);
                        }
                    }
                }
            }
        }

        private static void ParseRecord(XElement recordElement, RismDbContext dbContext)
        {
            foreach (var field in recordElement.Elements().Where(e => e.Name == "datafield"))
            {
                var tag = field.Attributes().FirstOrDefault(a => a.Name == "tag")?.Value;
                if (tag == null) continue;
                if (!FieldFactories.ContainsKey(tag)) continue;

                var entity = FieldFactories[tag]();
                var properties = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var property in properties)
                {
                    var subfieldAttribute = property.GetCustomAttribute<MarcSubfieldAttribute>();
                    if (subfieldAttribute == null) continue;

                    var value = field.Elements()
                        .FirstOrDefault(e => e.Attributes().FirstOrDefault(a => a.Name == "code")?.Value == subfieldAttribute.Code)?
                        .Value;
                    property.SetValue(entity, value);   //TODO: Type conversion, converter types, etc.
                }
                dbContext.Attach(entity);
                dbContext.SaveChanges();    //TODO: Bulk insert
            }
        }
    }
}