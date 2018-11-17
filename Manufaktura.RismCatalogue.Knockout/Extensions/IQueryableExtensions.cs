using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Manufaktura.RismCatalogue.Knockout.Extensions
{
    /// <summary>
    /// http://rion.io/2016/10/19/accessing-entity-framework-core-queries-behind-the-scenes-in-asp-net-core/
    /// </summary>
    public static class IQueryableExtensions
    {
        private static readonly Lazy<PropertyInfo> DatabaseDependenciesField = new Lazy<PropertyInfo>(() => typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies"));
        private static readonly Lazy<FieldInfo> DataBaseField = new Lazy<FieldInfo>(() => QueryCompilerTypeInfo.Value.DeclaredFields.Single(x => x.Name == "_database"));
        private static readonly Lazy<FieldInfo> QueryCompilerField = new Lazy<FieldInfo>(() => typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler"));
        private static readonly Lazy<TypeInfo> QueryCompilerTypeInfo = new Lazy<TypeInfo>(() => typeof(QueryCompiler).GetTypeInfo());
        private static readonly Lazy<FieldInfo> QueryModelGeneratorField = new Lazy<FieldInfo>(() => QueryCompilerTypeInfo.Value.DeclaredFields.First(x => x.Name == "_queryModelGenerator"));

        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var queryCompiler = (QueryCompiler)QueryCompilerField.Value.GetValue(query.Provider);
            var modelGenerator = (QueryModelGenerator)QueryModelGeneratorField.Value.GetValue(queryCompiler);
            var queryModel = modelGenerator.ParseQuery(query.Expression);
            var database = (IDatabase)DataBaseField.Value.GetValue(queryCompiler);
            var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.Value.GetValue(database);
            var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);

            var sb = new StringBuilder();
            var queryNumber = 1;
            foreach (var sqlQuery in modelVisitor.Queries)
            {
                sb.AppendLine("==================================");
                sb.AppendLine($" QUERY {queryNumber++}");
                sb.AppendLine("==================================");
                sb.AppendLine();
                sb.AppendLine(sqlQuery.ToString());
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static IEnumerable<object[]> RawSqlQuery(this DbContext context, string sql, params object[] parameters)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                int paramCount = 0;
                foreach (var parameter in parameters)
                {
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = $"@p{paramCount++}";
                    dbParameter.Value = parameter;
                    command.Parameters.Add(dbParameter);
                }
                if (command.Connection.State != ConnectionState.Open) command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object[] row = new object[reader.FieldCount];
                        reader.GetValues(row);
                        yield return row;
                    }
                }
                if (command.Connection.State == ConnectionState.Open) command.Connection.Close();
            }
        }
    }
}