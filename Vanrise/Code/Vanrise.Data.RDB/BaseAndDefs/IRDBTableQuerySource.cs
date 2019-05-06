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

        void GetColumnDefinition(IRDBTableQuerySourceGetColumnDefinitionContext context);

        List<string> GetColumnNames(IRDBTableQuerySourceGetColumnNamesContext context);

        void GetCreatedAndModifiedTime(IRDBTableQuerySourceGetCreatedAndModifiedTimeContext context);

        bool TryGetExpressionColumn(IRDBTableQuerySourceTryGetExpressionColumnContext context);

        void FinalizeBeforeResolveQuery(IRDBTableQuerySourceFinalizeBeforeResolveQueryContext context);
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

    public interface IRDBTableQuerySourceGetColumnNamesContext
    {
    }

    public class RDBTableQuerySourceGetColumnNamesContext : IRDBTableQuerySourceGetColumnNamesContext
    {
    }

    public interface IRDBTableQuerySourceGetCreatedAndModifiedTimeContext : IBaseRDBResolveQueryContext
    {
        string CreatedTimeColumnName { set; }

        string ModifiedTimeColumnName { set; }
    }

    public class RDBTableQuerySourceGetCreatedAndModifiedTimeContext : BaseRDBResolveQueryContext, IRDBTableQuerySourceGetCreatedAndModifiedTimeContext
    {
        public RDBTableQuerySourceGetCreatedAndModifiedTimeContext(IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {

        }

        public string CreatedTimeColumnName { get; set; }

        public string ModifiedTimeColumnName { get; set; }
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

    public interface IRDBTableQuerySourceGetColumnDefinitionContext
    {
        BaseRDBDataProvider DataProvider { get; }

        string ColumnName { get; }

        RDBTableColumnDefinition ColumnDefinition { set; }
    }

    public class RDBTableQuerySourceGetColumnDefinitionContext : BaseRDBResolveQueryContext, IRDBTableQuerySourceGetColumnDefinitionContext
    {
        public RDBTableQuerySourceGetColumnDefinitionContext(string columnName, IBaseRDBResolveQueryContext parentContext)
            : base(parentContext)
        {
            this.ColumnName = columnName;
        }

        public string ColumnName
        {
            get;
            private set;
        }

        public RDBTableColumnDefinition ColumnDefinition
        {
            get;
            set;
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

    public interface IRDBTableQuerySourceTryGetExpressionColumnContext
    {
        RDBQueryBuilderContext QueryBuilderContext { get; }

        string TableAlias { get; }

        string ColumnName { get; }

        RDBTableExpressionColumn ExpressionColumn { set; }
    }

    public class RDBTableQuerySourceTryGetExpressionColumnContext : IRDBTableQuerySourceTryGetExpressionColumnContext
    {
        public RDBTableQuerySourceTryGetExpressionColumnContext(RDBQueryBuilderContext queryBuilderContext, string tableAlias, string columnName)
        {
            this.QueryBuilderContext = queryBuilderContext;
            this.TableAlias = tableAlias;
            this.ColumnName = columnName;
        }

        public RDBQueryBuilderContext QueryBuilderContext
        {
            get;
            private set;
        }

        public string ColumnName
        {
            get;
            private set;
        }

        public RDBTableExpressionColumn ExpressionColumn
        {
            get;
            set;
        }

        public string TableAlias
        {
            get;
            private set;
        }
    }

    public interface IRDBTableQuerySourceFinalizeBeforeResolveQueryContext
    {
        BaseRDBDataProvider DataProvider { get; }

        string TableAlias { get; }
        
        RDBConditionContext QueryWhere { get; }
    }

    public class RDBTableQuerySourceFinalizeBeforeResolveQueryContext : IRDBTableQuerySourceFinalizeBeforeResolveQueryContext
    {
        Func<RDBConditionContext> _getQueryWhere;
        public RDBTableQuerySourceFinalizeBeforeResolveQueryContext(BaseRDBDataProvider dataProvider,string tableAlias, Func<RDBConditionContext> getQueryWhere)
        {
            this.DataProvider = dataProvider;
            this.TableAlias = tableAlias;
            this._getQueryWhere = getQueryWhere;
        }

        public BaseRDBDataProvider DataProvider
        {
            get;
            private set;
        }

        public string TableAlias
        {
            get;
            private set;
        }
        
        public RDBConditionContext QueryWhere
        {
            get
            {
                return _getQueryWhere();
            }
        }
    }
}
