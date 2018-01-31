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

        public abstract RDBResolvedSelectQuery ResolveSelectQuery(IRDBDataProviderResolveSelectQueryContext context);

        public abstract RDBResolvedNoDataQuery ResolveInsertQuery(IRDBDataProviderResolveInsertQueryContext context);

        public abstract RDBResolvedNoDataQuery ResolveUpdateQuery(IRDBDataProviderResolveUpdateQueryContext context);

        public abstract RDBResolvedNoDataQuery ResolveTempTableCreationQuery(IRDBDataProviderResolveTempTableCreationQueryContext context);

        public abstract void ExecuteReader(IRDBDataProviderExecuteReaderContext context);

        public abstract Object ExecuteScalar(IRDBDataProviderExecuteScalarContext context);

        public abstract int ExecuteNonQuery(IRDBDataProviderExecuteNonQueryContext context);
    }

    public interface IRDBDataProviderResolveSelectQueryContext : IBaseRDBResolveQueryContext
    {
        RDBSelectQuery SelectQuery { get; }
    }
    
    public class RDBDataProviderResolveSelectQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveSelectQueryContext
    {
        public RDBDataProviderResolveSelectQueryContext(RDBSelectQuery selectQuery, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(dataProvider, parameterValues)
        {
            this.SelectQuery = selectQuery;
        }

        public RDBDataProviderResolveSelectQueryContext(RDBSelectQuery selectQuery, IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {
            this.SelectQuery = selectQuery;
        }

        public RDBSelectQuery SelectQuery
        {
            get;
            private set;
        }
    }
    
    public class RDBResolvedSelectQuery
    {
        public string QueryText { get; set; }
    }

    public class RDBResolvedNoDataQuery
    {
        public string QueryText { get; set; }
    }

    public interface IRDBDataProviderResolveInsertQueryContext : IBaseRDBResolveQueryContext
    {
        RDBInsertQuery InsertQuery { get; }
    }
    public class RDBDataProviderResolveInsertQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveInsertQueryContext
    {
        public RDBDataProviderResolveInsertQueryContext(RDBInsertQuery insertQuery, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(dataProvider, parameterValues)
        {
            this.InsertQuery = insertQuery;
        }

        public RDBDataProviderResolveInsertQueryContext(RDBInsertQuery insertQuery, IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {
            this.InsertQuery = insertQuery;
        }

        public RDBInsertQuery InsertQuery
        {
            get;
            private set;
        }
    }
    public interface IRDBDataProviderResolveUpdateQueryContext : IBaseRDBResolveQueryContext
    {
        RDBUpdateQuery UpdateQuery { get; }
    }

    public class RDBDataProviderResolveUpdateQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveUpdateQueryContext
    {
        public RDBDataProviderResolveUpdateQueryContext(RDBUpdateQuery updateQuery, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(dataProvider, parameterValues)
        {
            this.UpdateQuery = updateQuery;
        }

        public RDBDataProviderResolveUpdateQueryContext(RDBUpdateQuery updateQuery, IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {
            this.UpdateQuery = updateQuery;
        }

        public RDBUpdateQuery UpdateQuery
        {
            get;
            private set;
        }
    }

    public interface IRDBDataProviderResolveTempTableCreationQueryContext : IBaseRDBResolveQueryContext
    {
        Dictionary<string, RDBTableColumnDefinition> Columns { get; }

        string TempTableName { set; }
    }

    public class RDBDataProviderResolveTempTableCreationQueryContext : BaseRDBResolveQueryContext, IRDBDataProviderResolveTempTableCreationQueryContext
    {
        public RDBDataProviderResolveTempTableCreationQueryContext(Dictionary<string, RDBTableColumnDefinition> columns, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(dataProvider, parameterValues)
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

    public interface IRDBDataProviderExecuteReaderContext
    {
        RDBResolvedSelectQuery ResolvedQuery { get; }

        Dictionary<string, Object> ParameterValues { get; }

        void OnReaderReady(IRDBDataReader reader);
    }

    public interface IRDBDataProviderExecuteScalarContext
    {
    }

    public interface IRDBDataProviderExecuteNonQueryContext
    {
        RDBResolvedNoDataQuery ResolvedQuery { get; }

        Dictionary<string, Object> ParameterValues { get; }
    }
}
