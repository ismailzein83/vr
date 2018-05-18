using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBBatchQuery<T> : BaseRDBQuery, IRDBBatchQuery<T>, IRDBBatchQueryReady<T>
    {
        List<IRDBQueryReady> _queries = new List<IRDBQueryReady>();

        T _parent;
        RDBQueryBuilderContext _queryBuilderContext;

        public RDBBatchQuery(T parent, RDBQueryBuilderContext queryBuilderContext)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
        }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            StringBuilder queryBuilder = new StringBuilder();
            foreach (var subQuery in _queries)
            {
                var subQueryContext = new RDBQueryGetResolvedQueryContext(context);
                RDBResolvedQuery resolvedSubQuery = subQuery.GetResolvedQuery(subQueryContext);
                queryBuilder.AppendLine(resolvedSubQuery.QueryText);
                queryBuilder.AppendLine();
            }

            return new RDBResolvedQuery
                {
                    QueryText = queryBuilder.ToString()
                };
        }

        RDBQueryContext<IRDBBatchQueryReady<T>> _lastQueryContext;
        public RDBQueryContext<IRDBBatchQueryReady<T>> AddQuery()
        {
            if (_lastQueryContext != null)
                this._queries.Add(_lastQueryContext.Query);
            _lastQueryContext = new RDBQueryContext<IRDBBatchQueryReady<T>>(this, _queryBuilderContext.CreateChildContext());
            return _lastQueryContext;
        }

        public IRDBBatchQueryReady<T> Foreach<Q>(IEnumerable<Q> items, Action<Q, IRDBBatchQuery<T>> action)
        {
            foreach(var item in items)
            {
                action(item, this);
            }
            return this;
        }

        public T EndBatchQuery()
        {
            if (_lastQueryContext != null)
                this._queries.Add(_lastQueryContext.Query);
            return _parent;
        }
    }

    public interface IRDBBatchQuery<T>
    {
        RDBQueryContext<IRDBBatchQueryReady<T>> AddQuery();

        IRDBBatchQueryReady<T> Foreach<Q>(IEnumerable<Q> items, Action<Q, IRDBBatchQuery<T>> action);
    }

    public interface IRDBBatchQueryReady<T>
    {
        RDBQueryContext<IRDBBatchQueryReady<T>> AddQuery();

        IRDBBatchQueryReady<T> Foreach<Q>(IEnumerable<Q> items, Action<Q, IRDBBatchQuery<T>> action);

        T EndBatchQuery();
    }
}
