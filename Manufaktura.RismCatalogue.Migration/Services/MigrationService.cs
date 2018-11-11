﻿using Manufaktura.LibraryStandards.Marc;
using Manufaktura.RismCatalogue.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Manufaktura.RismCatalogue.Migration.Services
{
    public class MigrationService
    {
        private readonly RismDbContext dbContext;

        public MigrationService(RismDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private static readonly Lazy<Dictionary<string, Func<MusicalSourceField>>> fieldFactories = new Lazy<Dictionary<string, Func<MusicalSourceField>>>(() =>
        {
            var dict = new Dictionary<string, Func<MusicalSourceField>>();
            foreach (var type in typeof(MusicalSourceField).Assembly.GetTypes())
            {
                var dataFieldAttribute = type.GetCustomAttribute<MarcDatafieldAttribute>();
                if (dataFieldAttribute == null) continue;

                var factoryMethod = Expression.Lambda(Expression.New(type)).Compile() as Func<MusicalSourceField>;
                dict.Add(dataFieldAttribute.Tag, factoryMethod);
            }
            return dict;
        });

        public static Dictionary<string, Func<MusicalSourceField>> FieldFactories => fieldFactories.Value;

        public void Migrate()
        {
            //var path = @"C:\Databases\rismAllMARCXMLexample\rism_130616_example.xml";
            var path = @"C:\Databases\rismAllMARCXML\rism_170316.xml";
            var maxRecords = 1000;
            var counter = 0;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = XmlReader.Create(fs, new XmlReaderSettings { IgnoreWhitespace = true }))
                {
                    while (reader.Read() && reader.Name != "record")
                    {
                        reader.MoveToContent();
                    }
                    while (counter < maxRecords)
                    {
                        var record = reader.ReadOuterXml();
                        if (string.IsNullOrWhiteSpace(record)) break;

                        var recordElement = XElement.Parse(record);
                        var dbRecord = ParseRecord(recordElement);
                        counter++;
                        Console.WriteLine($"Record {counter} ({dbRecord.Title} - {dbRecord.ComposerName}) added.");
                    }
                }
            }
        }

        private MusicalSource ParseRecord(XElement recordElement)
        {
            var record = new MusicalSource();
            foreach (var field in recordElement.Elements().Where(e => e.Name.LocalName == "controlfield"))
            {
                var tag = field.Attributes().FirstOrDefault(a => a.Name.LocalName == "tag")?.Value;
                if (tag == null) continue;

                if (tag == "001") record.Id = field.Value;
            }

            foreach (var field in recordElement.Elements().Where(e => e.Name.LocalName == "datafield"))
            {
                var tag = field.Attributes().FirstOrDefault(a => a.Name.LocalName == "tag")?.Value;
                if (tag == null) continue;
                if (!FieldFactories.ContainsKey(tag)) continue;

                var entity = FieldFactories[tag]();
                var properties = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var property in properties)
                {
                    var subfieldAttribute = property.GetCustomAttribute<MarcSubfieldAttribute>();
                    if (subfieldAttribute == null) continue;

                    var value = field.Elements()
                        .FirstOrDefault(e => e.Attributes().FirstOrDefault(a => a.Name.LocalName == "code")?.Value == subfieldAttribute.Code)?
                        .Value;
                    property.SetValue(entity, value);   //TODO: Type conversion, converter types, etc.
                }
                entity.MusicalSource = record;
                ExtractDataFromSubfields(record, entity);

                dbContext.Attach(entity);
            }

            dbContext.MusicalSources.Add(record);
            dbContext.SaveChanges();    //TODO: Bulk insert
            return record;
        }

        private static void ExtractDataFromSubfields(MusicalSource record, MusicalSourceField entity)
        {
            var composer = entity as Person;
            if (composer != null)
            {
                record.ComposerName = composer.PersonalName;
                record.ComposerDates = composer.Dates;
            }

            var uniformTitle = entity as UniformTitle;
            if (uniformTitle != null)
            {
                record.Title = uniformTitle.Title;
                record.FormSubheading = uniformTitle.FormSubheading;
                record.MediumOfPerformance = uniformTitle.MediumOfPerformance;
                record.PartOrSectionNumber = uniformTitle.PartOrSectionNumber;
            }
        }
    }
}