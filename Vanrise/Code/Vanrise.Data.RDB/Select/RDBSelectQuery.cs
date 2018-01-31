using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSelectQuery : BaseRDBNoDataQuery, ISelectQueryJoined, ISelectQueryColumnsSelected, ISelectQueryFiltered, ISelectQueryGroupByDefined, ISelectQueryAggregateColumnsSelected, ISelectQuerySortDefined, IRDBTableQuerySource
    {
        public RDBSelectQuery(IRDBTableQuerySource table, long nbOfRecords)
            : this(table)
        {
            this.NbOfRecords = nbOfRecords;
        }

        public RDBSelectQuery(IRDBTableQuerySource table)
        {
            this.Table = table;
        }

        public RDBSelectQuery(string tableName, long nbOfRecords)
            : this(tableName)
        {
            this.NbOfRecords = nbOfRecords;
        }

        public RDBSelectQuery(string tableName)
        {
            this.Table = new RDBTableDefinitionQuerySource(tableName);
            this.Columns = new List<RDBSelectColumn>();
            this.Joins = new List<RDBJoin>();
        } 

        internal IRDBTableQuerySource Table
        {
            get;
            private set;
        }

        internal long? NbOfRecords
        {
            get;
            private set;
        }

        internal List<RDBSelectColumn> Columns
        {
            get;
            private set;
        }

        internal List<RDBJoin> Joins
        {
            get;
            private set;
        }

        internal BaseRDBCondition Condition
        {
            get;
            private set;
        }

        internal RDBGroupBy GroupBy { get; private set; }

        internal List<RDBSelectSortColumn> SortColumns { get; private set; }

        public RDBJoinContext<ISelectQueryJoined> StartJoin()
        {
            this.Joins = new List<RDBJoin>();
            return new RDBJoinContext<ISelectQueryJoined>(this, this.Joins);
        }

        public RDBSelectContext<ISelectQueryColumnsSelected> StartSelect()
        {
            this.Columns = new List<RDBSelectColumn>();
            return new RDBSelectContext<ISelectQueryColumnsSelected>(this, this.Columns, this.Table);
        }

        public RDBSelectAggregateContext<ISelectQueryAggregateColumnsSelected> StartSelectAggregates()
        {
            this.Columns = new List<RDBSelectColumn>();
            return new RDBSelectAggregateContext<ISelectQueryAggregateColumnsSelected>(this, this.Columns, this.Table);
        }

        public RDBConditionContext<ISelectQueryFiltered> Where()
        {
            return new RDBConditionContext<ISelectQueryFiltered>(this, (condition) => this.Condition = condition, this.Table);
        }

        public RDBGroupByContext<ISelectQueryGroupByDefined> StartGroupBy()
        {
            this.GroupBy = new RDBGroupBy { Columns = new List<RDBSelectColumn>(), AggregateColumns = new List<RDBSelectColumn>() };
            return new RDBGroupByContext<ISelectQueryGroupByDefined>(this, this.GroupBy, this.Table);
        }

        RDBSortContext<ISelectQuerySortDefined> ISelectQueryCanSort.Sort()
        {
            this.SortColumns = new List<RDBSelectSortColumn>();
            return new RDBSortContext<ISelectQuerySortDefined>(this, this.SortColumns, this.Table);
        }

        List<T> ISelectQueryReady.GetItems<T>(Func<IRDBDataReader, T> objectBuilder)
        {
            List<T> items = new List<T>();
            ((ISelectQueryReady)this).ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    items.Add(objectBuilder(reader));
                }
            });
            return items;
        }

        object ISelectQueryReady.ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        void ISelectQueryReady.ExecuteReader(Action<IRDBDataReader> onReaderReady)
        {
            Dictionary<string, Object> parameterValues;
            var resolvedQuery = ((ISelectQueryReady)this).ResolveQuery(out parameterValues);
            var context = new RDBDataProviderExecuteReaderContext(resolvedQuery, parameterValues, onReaderReady);
            this.DataProvider.ExecuteReader(context);
        }

        public RDBResolvedSelectQuery ResolveQuery(IBaseRDBResolveQueryContext parentContext)
        {
            var context = new RDBDataProviderResolveSelectQueryContext(this, parentContext, false);
            return this.DataProvider.ResolveSelectQuery(context);
        }

        RDBResolvedSelectQuery ISelectQueryReady.ResolveQuery(out Dictionary<string, Object> parameterValues)
        {
            parameterValues = new Dictionary<string, object>();
            var context = new RDBDataProviderResolveSelectQueryContext(this, this.DataProvider, parameterValues);
            return this.DataProvider.ResolveSelectQuery(context);
        }

        public string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context)
        {
            RDBResolvedSelectQuery resolvedQuery = ResolveQuery(context);
            return resolvedQuery.QueryText;
        }

        #region Private Classes

        private class RDBDataProviderExecuteReaderContext : IRDBDataProviderExecuteReaderContext
        {
            Action<IRDBDataReader> _onReaderReady;
            public RDBDataProviderExecuteReaderContext(RDBResolvedSelectQuery resolvedQuery, Dictionary<string, object> parameterValues, Action<IRDBDataReader> onReaderReady)
            {
                this.ResolvedQuery = resolvedQuery;
                this.ParameterValues = parameterValues;
                _onReaderReady = onReaderReady;
            }

            public RDBResolvedSelectQuery ResolvedQuery
            {
                get;
                private set;
            }

            public void OnReaderReady(IRDBDataReader reader)
            {
                _onReaderReady(reader);
            }


            public Dictionary<string, object> ParameterValues
            {
                get;
                private set;
            }
        }


        #endregion

        public string GetDescription()
        {
            return String.Format("Inline Query on table '{0}'", this.Table.GetDescription());
        }

        public string GetDBColumnName(IRDBTableQuerySourceGetDBColumnNameContext context)
        {
            return context.ColumnName;
        }

        string _queryAsTableSourceUniqueName = Guid.NewGuid().ToString();
        public string GetUniqueName()
        {
            return _queryAsTableSourceUniqueName;
        }

        public override RDBResolvedNoDataQuery GetResolvedQuery(IRDBNoDataQueryGetResolvedQueryContext context)
        {
            var resolvedSelectQuery = ResolveQuery(context);
            return new RDBResolvedNoDataQuery
            {
                QueryText = resolvedSelectQuery.QueryText
            };
        }
    }

    public class RDBSelectColumn
    {
        public BaseRDBExpression Expression { get; set; }

        public string Alias { get; set; }
    }

    public enum RDBSortDirection { ASC = 0, DESC = 1 }

    public class RDBSelectSortColumn
    {
        /// <summary>
        /// either Expression or ColumnAlias should be filled
        /// </summary>
        public BaseRDBExpression Expression { get; set; }

        public string ColumnAlias { get; set; }

        public RDBSortDirection SortDirection { get; set; }
    }

    public interface ISelectQueryReady
    {
        List<T> GetItems<T>(Func<IRDBDataReader, T> objectBuilder);

        Object ExecuteScalar();

        void ExecuteReader(Action<IRDBDataReader> onReaderReady);

        RDBResolvedSelectQuery ResolveQuery(out Dictionary<string, Object> parameterValues);
    }

    public interface ISelectQueryJoined : ISelectQueryCanFilter, ISelectQueryCanSelectColumns, ISelectQueryCanGroupBy, ISelectQueryCanSelectAggregate
    {
        
    }

    public interface ISelectQueryColumnsSelected : ISelectQueryReady, ISelectQueryCanSort
    {
       
    }

    public interface ISelectQueryFiltered : ISelectQueryCanSelectColumns, ISelectQueryCanGroupBy, ISelectQueryCanSelectAggregate
    {

    }

    public interface ISelectQueryGroupByDefined : ISelectQueryReady, ISelectQueryCanSort
    {
    }

    public interface ISelectQuerySortDefined : ISelectQueryReady
    {
    }

    public interface ISelectQueryAggregateColumnsSelected : ISelectQueryReady, ISelectQueryCanSort
    {

    }

    public interface ISelectQueryCanSelectColumns
    {
        RDBSelectContext<ISelectQueryColumnsSelected> StartSelect();
    }

    public interface ISelectQueryCanFilter
    {
        RDBConditionContext<ISelectQueryFiltered> Where();
    }

    public interface ISelectQueryCanGroupBy
    {
        RDBGroupByContext<ISelectQueryGroupByDefined> StartGroupBy();
    }

    public interface ISelectQueryCanJoin
    {
        RDBJoinContext<ISelectQueryJoined> StartJoin();
    }

    public interface ISelectQueryCanSort
    {
        RDBSortContext<ISelectQuerySortDefined> Sort();
    }

    public interface ISelectQueryCanSelectAggregate
    {
        RDBSelectAggregateContext<ISelectQueryAggregateColumnsSelected> StartSelectAggregates();
    }
}
