using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBTempTableQuery : BaseRDBNoDataQuery, IRDBTableQuerySource
    {
        Dictionary<string, RDBTableColumnDefinition> _columns;
        string _tempTableName;
        RDBResolvedNoDataQuery _resolvedQuery;

        public RDBTempTableQuery(Dictionary<string, RDBTableColumnDefinition> columns)
        {
            this._columns = columns;
            Dictionary<string, object> parameterValues = new Dictionary<string,object>();
            var resolveQueryContext = new RDBDataProviderResolveTempTableCreationQueryContext(columns, this.DataProvider, parameterValues);
            _resolvedQuery = this.DataProvider.ResolveTempTableCreationQuery(resolveQueryContext);
            _tempTableName = resolveQueryContext.TempTableName;
        }
        
        public override RDBResolvedNoDataQuery GetResolvedQuery(IRDBNoDataQueryGetResolvedQueryContext context)
        {
            return _resolvedQuery;
        }

        

        public string GetUniqueName()
        {
            return _tempTableName;
        }

        public string GetDescription()
        {
            return String.Format("Temp Table '{0}'", GetUniqueName());
        }

        public string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context)
        {
            return _tempTableName;
        }

        public string GetDBColumnName(IRDBTableQuerySourceGetDBColumnNameContext context)
        {
            RDBTableColumnDefinition columnDef;
            if (!_columns.TryGetValue(context.ColumnName, out columnDef))
                throw new Exception(String.Format("Column '{0}' not found", context.ColumnName));
            return columnDef.DBColumnName;
        }
    }
}