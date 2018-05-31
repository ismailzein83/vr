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
        Dictionary<string, RDBTableColumnDefinition> _columns;
        string _tempTableName;

        public RDBTempTableQuery(Dictionary<string, RDBTableColumnDefinition> columns)
        {
            this._columns = columns;
        }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            Dictionary<string, object> parameterValues = new Dictionary<string, object>();
            var resolveQueryContext = new RDBDataProviderResolveTempTableCreationQueryContext(_columns, context);
            var resolvedQuery = context.DataProvider.ResolveTempTableCreationQuery(resolveQueryContext);
            _tempTableName = resolveQueryContext.TempTableName;
            return resolvedQuery;
        }

        

        public string GetUniqueName()
        {
            _tempTableName.ThrowIfNull("_tempTableName");
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
            RDBTableColumnDefinition columnDef;
            if (!_columns.TryGetValue(context.ColumnName, out columnDef))
                throw new Exception(String.Format("Column '{0}' not found", context.ColumnName));
            return RDBSchemaManager.Current.GetColumnDBName(context.DataProvider, context.ColumnName, columnDef);
        }


        public void GetIdColumnInfo(IRDBTableQuerySourceGetIdColumnInfoContext context)
        {
            throw new NotImplementedException();
        }
    }
}