using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBTempTableQuery : BaseRDBQuery, IRDBTableQuerySource
    {
        Dictionary<string, RDBTempTableColumnDefinition> _columns = new Dictionary<string, RDBTempTableColumnDefinition>();
        string _tempTableName;

        RDBQueryBuilderContext _queryBuilderContext;

        internal RDBTempTableQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        public void AddColumn(string columnName, RDBDataType type)
        {
            AddColumn(columnName, type, false);
        }

        public void AddColumn(string columnName, RDBDataType type, bool isPrimaryKey)
        {
            AddColumn(columnName, type, null, null, isPrimaryKey);
        }

        public void AddColumn(string columnName, RDBDataType type, int? size, int? precision)
        {
            AddColumn(columnName, type, size, precision, false);
        }

        public void AddColumn(string columnName, RDBDataType type, int? size, int? precision, bool isPrimaryKey)
        {
            this._columns.Add(columnName, new RDBTempTableColumnDefinition
            {
                ColumnDefinition = new RDBTableColumnDefinition
                    {
                        DBColumnName = columnName,
                        DataType = type,
                        Size = size,
                        Precision = precision
                    },
                IsPrimaryKey = isPrimaryKey
            });
        }

        public void AddColumnsFromTable(string tableName)
        {
            var tableDefinition =RDBSchemaManager.Current.GetTableDefinitionWithValidate(_queryBuilderContext.DataProvider, tableName);
            foreach(var columnEntry in tableDefinition.Columns)
            {
                this._columns.Add(columnEntry.Key, new RDBTempTableColumnDefinition { ColumnDefinition = columnEntry.Value, IsPrimaryKey = tableDefinition.IdColumnName == columnEntry.Key });
            }
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            Dictionary<string, object> parameterValues = new Dictionary<string, object>();
            var resolveQueryContext = new RDBDataProviderResolveTempTableCreationQueryContext(_columns, context);
            var resolvedQuery = context.DataProvider.ResolveTempTableCreationQuery(resolveQueryContext);
            _tempTableName = resolveQueryContext.TempTableName;
            context.AddTempTableName(_tempTableName);
            return resolvedQuery;
        }

        public string GetUniqueName()
        {
            if (_tempTableName == null)
                throw new Exception("Create Temp Table is not added to the query");
            return _tempTableName;
        }

        public string GetDescription()
        {
            return "Temp Table";
        }

        public string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context)
        {
            _tempTableName.ThrowIfNull("_tempTableName");
            return _tempTableName;
        }

        public string GetDBColumnName(IRDBTableQuerySourceGetDBColumnNameContext context)
        {
            RDBTempTableColumnDefinition columnDef;
            if (!_columns.TryGetValue(context.ColumnName, out columnDef))
                throw new Exception(String.Format("Column '{0}' not found", context.ColumnName));
            return RDBSchemaManager.GetColumnDBName(context.DataProvider, context.ColumnName, columnDef.ColumnDefinition);
        }


        public void GetIdColumnInfo(IRDBTableQuerySourceGetIdColumnInfoContext context)
        {
            throw new NotImplementedException();
        }


        public List<string> GetColumnNames(IRDBTableQuerySourceGetColumnNamesContext context)
        {
            return _columns.Keys.ToList();
        }


        public void GetCreatedAndModifiedTime(IRDBTableQuerySourceGetCreatedAndModifiedTimeContext context)
        {            
        }


        public void GetColumnDefinition(IRDBTableQuerySourceGetColumnDefinitionContext context)
        {
            RDBTempTableColumnDefinition columnDef;
            if (!_columns.TryGetValue(context.ColumnName, out columnDef))
                throw new Exception(String.Format("Column '{0}' not found", context.ColumnName));
            context.ColumnDefinition = columnDef.ColumnDefinition;
        }
    }

    public class RDBTempTableColumnDefinition
    {
        public RDBTableColumnDefinition ColumnDefinition { get; set; }

        public bool IsPrimaryKey { get; set; }
    }
}