using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSelectQuery : BaseRDBQuery, ISelectQueryJoined, ISelectQueryColumnsSelected, ISelectQueryFiltered, IRDBTableQuerySource
    {
        public RDBSelectQuery(IRDBTableQuerySource table)
        {
            this._table = table;
        }

        IRDBTableQuerySource _table;
        internal IRDBTableQuerySource Table
        {
            get
            {
                return _table;
            }
        }

        List<RDBSelectColumn> _columns = new List<RDBSelectColumn>();
        internal List<RDBSelectColumn> Columns
        {
            get
            {
                return _columns;
            }
        }

        List<RDBJoin> _joins = new List<RDBJoin>();
        internal List<RDBJoin> Joins
        {
            get
            {
                return _joins;
            }
        }

        BaseRDBCondition _condition;
        internal BaseRDBCondition Condition
        {
            get
            {
                return _condition;
            }
        }

        public RDBJoinContext<ISelectQueryJoined> StartJoin()
        {
            return new RDBJoinContext<ISelectQueryJoined>(this, this._joins);
        }

        public RDBSelectContext<ISelectQueryColumnsSelected> StartSelect()
        {
            return new RDBSelectContext<ISelectQueryColumnsSelected>(this, this._columns, this._table);
        }

        RDBConditionContext<ISelectQueryFiltered> ISelectQueryColumnsSelected.Where()
        {
            return new RDBConditionContext<ISelectQueryFiltered>(this, (condition) => this._condition = condition);
        }

        public List<T> GetItems<T>(Func<IRDBDataReader, T> objectBuilder)
        {
            List<T> items = new List<T>();
            ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    items.Add(objectBuilder(reader));
                }
            });
            return items;
        }

        public object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        public void ExecuteReader(Action<IRDBDataReader> onReaderReady)
        {
            var resolvedQuery = ResolveQuery();
            var context = new RDBDataProviderExecuteReaderContext(resolvedQuery, onReaderReady);
            this.DataProvider.ExecuteReader(context);
        }

        public RDBResolvedSelectQuery ResolveQuery()
        {
            var context = new RDBDataProviderResolveSelectQueryContext(this, this.DataProvider);
            return this.DataProvider.ResolveSelectQuery(context);
        }

        public string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context)
        {
            RDBResolvedSelectQuery resolvedQuery = ResolveQuery();
            if (resolvedQuery.ParameterValues != null && resolvedQuery.ParameterValues.Count > 0)
            {
                foreach (var prm in resolvedQuery.ParameterValues)
                {
                    context.ParameterValues.Add(prm.Key, prm.Value);
                }
            }
            return resolvedQuery.QueryText;
        }

        #region Private Classes

        private class RDBDataProviderResolveSelectQueryContext : RDBDataProviderResolveQueryContext, IRDBDataProviderResolveSelectQueryContext
        {
            public RDBDataProviderResolveSelectQueryContext(RDBSelectQuery selectQuery, BaseRDBDataProvider dataProvider)
                : base(dataProvider)
            {
                this.SelectQuery = selectQuery;
            }
            public RDBSelectQuery SelectQuery
            {
                get;
                private set;
            }
        }

        private class RDBDataProviderExecuteReaderContext : IRDBDataProviderExecuteReaderContext
        {
            Action<IRDBDataReader> _onReaderReady;
            public RDBDataProviderExecuteReaderContext(RDBResolvedSelectQuery resolvedQuery, Action<IRDBDataReader> onReaderReady)
            {
                this.ResolvedQuery = resolvedQuery;
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
        }


        #endregion

        public string GetDescription()
        {
            return String.Format("Inline Query on table '{0}'", this.Table.GetDescription());
        }
    }

    public class RDBSelectColumn
    {
        public BaseRDBExpression Expression { get; set; }

        public string Alias { get; set; }
    }

    public interface ISelectQueryJoined
    {
        RDBSelectContext<ISelectQueryColumnsSelected> StartSelect();
    }

    public interface ISelectQueryColumnsSelected : ISelectQueryReady
    {
        RDBConditionContext<ISelectQueryFiltered> Where();
    }

    public interface ISelectQueryFiltered : ISelectQueryReady
    {

    }

    public interface ISelectQueryReady
    {
        List<T> GetItems<T>(Func<IRDBDataReader, T> objectBuilder);

        Object ExecuteScalar();

        void ExecuteReader(Action<IRDBDataReader> onReaderReady);

        RDBResolvedSelectQuery ResolveQuery();
    }

    public class RDBSelectQueryColumnsSelected
    {
        public RDBSelectQueryColumnsSelected(RDBSelectQuery parentQuery)
        {

        }
    }
}
