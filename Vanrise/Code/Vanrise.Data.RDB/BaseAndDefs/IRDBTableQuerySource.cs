using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public interface IRDBTableQuerySource
    {
        string GetUniqueName();

        string GetDescription();

        string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context);

        string GetDBColumnName(IRDBTableQuerySourceGetDBColumnNameContext context);
    }

    public interface IRDBTableQuerySourceToDBQueryContext : IBaseRDBResolveQueryContext
    {        
    }

    public class RDBTableQuerySourceToDBQueryContext : BaseRDBResolveQueryContext, IRDBTableQuerySourceToDBQueryContext
    {
        public RDBTableQuerySourceToDBQueryContext(BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(queryContext, dataProvider, parameterValues)
        {            
        }

        public RDBTableQuerySourceToDBQueryContext(IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {

        }
    }

    public interface IRDBTableQuerySourceGetDBColumnNameContext : IBaseRDBResolveQueryContext
    {
        string ColumnName { get; }
    }

    public class RDBTableQuerySourceGetDBColumnNameContext : BaseRDBResolveQueryContext, IRDBTableQuerySourceGetDBColumnNameContext
    {
        public RDBTableQuerySourceGetDBColumnNameContext(BaseRDBQueryContext queryContext, string columnName, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(queryContext, dataProvider, parameterValues)
        {
            this.ColumnName = columnName;
        }

        public RDBTableQuerySourceGetDBColumnNameContext(string columnName, IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {
            this.ColumnName = columnName;
        }

        public string ColumnName
        {
            get;
            private set;
        }
    }
}
