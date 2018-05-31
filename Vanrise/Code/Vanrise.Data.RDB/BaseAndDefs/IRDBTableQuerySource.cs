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

        void GetIdColumnInfo(IRDBTableQuerySourceGetIdColumnInfoContext context);
    }

    public interface IRDBTableQuerySourceToDBQueryContext : IBaseRDBResolveQueryContext
    {        
    }

    public class RDBTableQuerySourceToDBQueryContext : BaseRDBResolveQueryContext, IRDBTableQuerySourceToDBQueryContext
    {
        public RDBTableQuerySourceToDBQueryContext(IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {

        }
    }

    public interface IRDBTableQuerySourceGetDBColumnNameContext : IBaseRDBResolveQueryContext
    {
        string ColumnName { get; }
    }

    public class RDBTableQuerySourceGetDBColumnNameContext : BaseRDBResolveQueryContext, IRDBTableQuerySourceGetDBColumnNameContext
    {
        public RDBTableQuerySourceGetDBColumnNameContext(string columnName, IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {
            this.ColumnName = columnName;
        }

        public string ColumnName
        {
            get;
            private set;
        }
    }

    public interface IRDBTableQuerySourceGetIdColumnInfoContext
    {
        BaseRDBDataProvider DataProvider { get; }

        string IdColumnName { set; }

        RDBTableColumnDefinition IdColumnDefinition { set; }
    }

    public class RDBTableQuerySourceGetIdColumnInfoContext : IRDBTableQuerySourceGetIdColumnInfoContext
    {
        public RDBTableQuerySourceGetIdColumnInfoContext(BaseRDBDataProvider dataProvider)
        {
            this.DataProvider = dataProvider;
        }
        public BaseRDBDataProvider DataProvider
        {
            get;
            private set;
        }

        public string IdColumnName
        {
            get;
            set;
        }

        public RDBTableColumnDefinition IdColumnDefinition
        {
            get;
            set;
        }
    }

}
