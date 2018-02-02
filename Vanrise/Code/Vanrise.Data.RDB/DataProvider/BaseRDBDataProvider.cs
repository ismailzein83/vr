using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBDataProvider
    {
        public abstract string UniqueName { get; }

        public abstract string ParameterNamePrefix { get; }

        public abstract string NowDateTimeFunction { get; }

        public abstract RDBResolvedQuery ResolveSelectQuery(IRDBDataProviderResolveSelectQueryContext context);

        public abstract RDBResolvedQuery ResolveInsertQuery(IRDBDataProviderResolveInsertQueryContext context);

        public abstract RDBResolvedQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context);

        public abstract RDBResolvedQuery ResolveTempTableCreationQuery(IRDBDataProviderResolveTempTableCreationQueryContext context);

        public abstract void ExecuteReader(IRDBDataProviderExecuteReaderContext context);

        public abstract Object ExecuteScalar(IRDBDataProviderExecuteScalarContext context);

        public abstract int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context);
    }

    public interface IRDBDataProviderResolveSelectQueryContext : IBaseRDBResolveQueryContext
    {
        IRDBTableQuerySource Table
        {
            get;
        }

        long? NbOfRecords
        {
            get;
        }

        List<RDBSelectColumn> Columns
        {
            get;
        }

        List<RDBJoin> Joins
        {
            get;
        }

        BaseRDBCondition Condition
        {
            get;
        }

        RDBGroupBy GroupBy { get; }

        List<RDBSelectSortColumn> SortColumns { get; }
    }
    
    public class RDBDataProviderResolveSelectQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveSelectQueryContext
    {
        public RDBDataProviderResolveSelectQueryContext(IRDBTableQuerySource table, long? nbOfRecords, List<RDBSelectColumn> columns, List<RDBJoin> joins,
            BaseRDBCondition condition, RDBGroupBy groupBy, List<RDBSelectSortColumn> sortColumns, BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(queryContext, dataProvider, parameterValues)
        {
            this.Table = table;
            this.NbOfRecords = nbOfRecords;
            this.Columns = columns;
            this.Joins = joins;
            this.Condition = condition;
            this.GroupBy = groupBy;
            this.SortColumns = sortColumns;
        }

        public RDBDataProviderResolveSelectQueryContext(IRDBTableQuerySource table, long? nbOfRecords, List<RDBSelectColumn> columns, List<RDBJoin> joins,
            BaseRDBCondition condition, RDBGroupBy groupBy, List<RDBSelectSortColumn> sortColumns, IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {
            this.Table = table;
            this.NbOfRecords = nbOfRecords;
            this.Columns = columns;
            this.Joins = joins;
            this.Condition = condition;
            this.GroupBy = groupBy;
            this.SortColumns = sortColumns;
        }

        public IRDBTableQuerySource Table
        {
            get;
            private set;
        }

        public long? NbOfRecords
        {
            get;
            private set;
        }

        public List<RDBSelectColumn> Columns
        {
            get;
            private set;
        }

        public List<RDBJoin> Joins
        {
            get;
            private set;
        }

        public BaseRDBCondition Condition
        {
            get;
            private set;
        }

        public RDBGroupBy GroupBy { get; private set; }

        public List<RDBSelectSortColumn> SortColumns { get; private set; }
    }
    
    public class RDBResolvedQuery
    {
        public string QueryText { get; set; }
    }

    public interface IRDBDataProviderResolveInsertQueryContext : IBaseRDBResolveQueryContext
    {
        IRDBTableQuerySource Table { get; }

        List<RDBInsertColumn> ColumnValues { get;  }

        string IdOutputParameterName { get; }
    }
    public class RDBDataProviderResolveInsertQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveInsertQueryContext
    {
        public RDBDataProviderResolveInsertQueryContext(IRDBTableQuerySource table, List<RDBInsertColumn> columnValues, string idOutputParameterName, BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(queryContext, dataProvider, parameterValues)
        {
            this.Table = table;
            this.ColumnValues = columnValues;
            this.IdOutputParameterName = idOutputParameterName;
        }

        public RDBDataProviderResolveInsertQueryContext(IRDBTableQuerySource table, List<RDBInsertColumn> columnValues, string idOutputParameterName, IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {
            this.Table = table;
            this.ColumnValues = columnValues;
            this.IdOutputParameterName = idOutputParameterName;
        }

        public IRDBTableQuerySource Table { get; set; }

        public List<RDBInsertColumn> ColumnValues { get; private set; }
        
        public string IdOutputParameterName { get; private set; }
    }
    public interface IRDBDataProviderResolveUpdateQueryContext : IBaseRDBResolveQueryContext
    {
        IRDBTableQuerySource Table { get; }

        List<RDBUpdateColumn> ColumnValues { get; }

        BaseRDBCondition Condition { get; }

        List<RDBJoin> Joins { get; }
    }

    public class RDBDataProviderResolveUpdateQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveUpdateQueryContext
    {
        public RDBDataProviderResolveUpdateQueryContext(IRDBTableQuerySource table, List<RDBUpdateColumn> columnValues, BaseRDBCondition condition, List<RDBJoin> joins, BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(queryContext, dataProvider, parameterValues)
        {
            this.Table = table;
            this.ColumnValues = columnValues;
            this.Condition = condition;
            this.Joins = joins;
        }

        public RDBDataProviderResolveUpdateQueryContext(IRDBTableQuerySource table, List<RDBUpdateColumn> columnValues, BaseRDBCondition condition, List<RDBJoin> joins, IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {
            this.Table = table;
            this.ColumnValues = columnValues;
            this.Condition = condition;
            this.Joins = joins;
        }

        public IRDBTableQuerySource Table { get; private set; }

        public List<RDBUpdateColumn> ColumnValues { get; private set; }

        public BaseRDBCondition Condition { get; private set; }

        public List<RDBJoin> Joins { get; private set; }
    }

    public interface IRDBDataProviderResolveTempTableCreationQueryContext : IBaseRDBResolveQueryContext
    {
        Dictionary<string, RDBTableColumnDefinition> Columns { get; }

        string TempTableName { set; }
    }

    public class RDBDataProviderResolveTempTableCreationQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveTempTableCreationQueryContext
    {
        public RDBDataProviderResolveTempTableCreationQueryContext(Dictionary<string, RDBTableColumnDefinition> columns, BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(queryContext, dataProvider, parameterValues)
        {
            this.Columns = columns;

        }

        public RDBDataProviderResolveTempTableCreationQueryContext(Dictionary<string, RDBTableColumnDefinition> columns, IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {
            this.Columns = columns;
        }


        public Dictionary<string, RDBTableColumnDefinition> Columns
        {
            get;
            private set;
        }


        public string TempTableName
        {
            set;
            get;
        }
    }

    public interface IBaseRDBDataProviderExecuteQueryContext
    {
        RDBResolvedQuery ResolvedQuery { get; }

        Dictionary<string, Object> ParameterValues { get; }

        Dictionary<string, RDBDataType> OutputParameters { get; }

        Dictionary<string, Object> OutputParameterValues { get; }
    }

    public abstract class BaseRDBDataProviderExecuteQueryContext : IBaseRDBDataProviderExecuteQueryContext
    {
        public BaseRDBDataProviderExecuteQueryContext(RDBResolvedQuery resolvedQuery, Dictionary<string, object> parameterValues, Dictionary<string, RDBDataType> outputParameters)
        {
            this.ResolvedQuery = resolvedQuery;
            this.ParameterValues = parameterValues;
            this.OutputParameters = outputParameters;
            this.OutputParameterValues = new Dictionary<string, object>();
        }

        public RDBResolvedQuery ResolvedQuery
        {
            get;
            private set;
        }

        public Dictionary<string, object> ParameterValues
        {

            get;
            private set;
        }

        public Dictionary<string, RDBDataType> OutputParameters
        {
            get;
            private set;
        }

        public Dictionary<string, object> OutputParameterValues
        {
            get;
            private set;
        }
    }


    public interface IRDBDataProviderExecuteReaderContext : IBaseRDBDataProviderExecuteQueryContext
    {
        void OnReaderReady(IRDBDataReader reader);
    }

    public interface IRDBDataProviderExecuteScalarContext : IBaseRDBDataProviderExecuteQueryContext
    {
    }

    public interface IRDBDataProviderExecuteNonQueryContext : IBaseRDBDataProviderExecuteQueryContext
    {
        
    }
}
