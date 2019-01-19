using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace PROTR.Core
{
    public enum DbDialectEnum
    {
        SQLServer,
        PostgreSQL,
        SQLite,
        MySQL,
    }

    public class DbDialect
    {
        private static readonly Lazy<DbDialect>[] _instance
            = { new Lazy<DbDialect>(() => new DbDialect(DbDialectEnum.SQLServer)),
                new Lazy<DbDialect>(() => new DbDialect(DbDialectEnum.PostgreSQL)),
                new Lazy<DbDialect>(() => new DbDialect(DbDialectEnum.SQLite)),
                new Lazy<DbDialect>(() => new DbDialect(DbDialectEnum.MySQL)) };

        public DbDialectEnum Dialect { get; }
        public string Encapsulation { get; }
        public string GetListCountSql { get; }
        public string GetPagedListSql { get; }
        public string GetFromToListSql { get; }

        private static DbDialect Retreive(DbDialectEnum dialect)
        {
            return new DbDialect(dialect);
        }

        private DbDialect(DbDialectEnum dialect)
        {
            switch (dialect)
            {
                case DbDialectEnum.PostgreSQL:
                    Dialect = DbDialectEnum.PostgreSQL;
                    Encapsulation = "{0}";
                    GetListCountSql = "SELECT COUNT(*) FROM (SELECT {SelectColumns} FROM {FromClause} {WhereClause} {GroupByClause}) AS u";
                    GetPagedListSql = "Select {SelectColumns} from {FromClause} {WhereClause} {GroupByClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
                    GetFromToListSql = "Select {SelectColumns} from {FromClause} {WhereClause} {GroupByClause} Order By {OrderBy} LIMIT {RowCount} OFFSET ({FromRecord})";
                    break;
                case DbDialectEnum.SQLite:
                    Dialect = DbDialectEnum.SQLite;
                    Encapsulation = "{0}";
                    GetListCountSql = "SELECT COUNT(*) FROM (SELECT {SelectColumns} FROM {FromClause} {WhereClause} {GroupByClause}) AS u";
                    GetPagedListSql = "Select {SelectColumns} from {FromClause} {WhereClause} {GroupByClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
                    GetFromToListSql = "Select {SelectColumns} from {FromClause} {WhereClause} {GroupByClause} Order By {OrderBy} LIMIT {RowCount} OFFSET ({FromRecord})";
                    break;
                case DbDialectEnum.MySQL:
                    Dialect = DbDialectEnum.MySQL;
                    Encapsulation = "`{0}`";
                    GetListCountSql = "SELECT COUNT(*) FROM (SELECT {SelectColumns} FROM {FromClause} {WhereClause} {GroupByClause}) AS u";
                    GetPagedListSql = "Select {SelectColumns} from {FromClause} {WhereClause} {GroupByClause} Order By {OrderBy} LIMIT {Offset},{RowsPerPage}";
                    GetFromToListSql = "Select {SelectColumns} from {FromClause} {WhereClause} {GroupByClause} Order By {OrderBy} LIMIT {FromRecord},{RowCount}";
                    break;
                default:
                    Dialect = DbDialectEnum.SQLServer;
                    Encapsulation = "[{0}]";
                    GetListCountSql = "SELECT COUNT(*) FROM (SELECT {SelectColumns} FROM {FromClause} {WhereClause} {GroupByClause}) AS u";
                    GetPagedListSql = "SELECT {SelectNamedColumns} FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {FromClause} {WhereClause} {GroupByClause}) AS u WHERE PagedNUMBER BETWEEN (({PageNumber}-1) * {RowsPerPage} + 1) AND ({PageNumber} * {RowsPerPage})";
                    GetFromToListSql = "SELECT {SelectNamedColumns} FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {FromClause} {WhereClause} {GroupByClause}) AS u WHERE PagedNUMBER BETWEEN ({FromRecord} + 1) AND ({FromRecord} + {RowCount})";
                    break;
            }
        }

        public static DbDialect Instance(DbDialectEnum dialect)
        {
            return _instance[(int)dialect].Value;
        }

        public string SQLAllColumns(List<PropertyDefinition> properties)
        {
            StringBuilder sb = new StringBuilder();
            var addedAny = false;

            foreach (PropertyDefinition prop in properties)
            {
                if (prop.IsDBField)
                {
                    if (addedAny)
                    {
                        sb.Append(",");
                    }
                    sb.Append(Encapsulate(prop.FieldName));

                    addedAny = true;
                }
            }

            return sb.ToString();
        }

        public string SQLWherePrimaryKey(List<PropertyDefinition> properties)
        {
            StringBuilder sb = new StringBuilder();
            var addedAny = false;

            foreach (PropertyDefinition prop in properties)
            {
                if (prop.IsDBField && prop.IsPrimaryKey)
                {
                    if (addedAny)
                    {
                        sb.Append(" AND ");
                    }
                    sb.Append(Encapsulate(prop.FieldName) + "=@" + prop.FieldName);

                    addedAny = true;
                }
            }

            return sb.ToString();
        }

        public string Encapsulate(string databaseword)
        {
            if (Dialect == DbDialectEnum.SQLServer && databaseword.Contains("."))
            {
                string[] parts = databaseword.Split('.');

                // No more than 2.

                return string.Format(Encapsulation, parts[0]) + "."
                    + string.Format(Encapsulation, parts[1]);
            }

            return string.Format(Encapsulation, databaseword);
        }
    }
}
