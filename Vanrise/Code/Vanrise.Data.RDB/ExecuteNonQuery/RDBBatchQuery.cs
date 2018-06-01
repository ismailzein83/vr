using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBBatchQuery<T> : BaseRDBQuery, IRDBBatchQuery<T>, IRDBBatchQueryQueryAdded<T>
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

        RDBQueryContext<IRDBBatchQueryQueryAdded<T>> _lastQueryContext;
        public RDBQueryContext<IRDBBatchQueryQueryAdded<T>> AddQuery()
        {
            if (_lastQueryContext != null && _lastQueryContext.Query != null)
                this._queries.Add(_lastQueryContext.Query);
            _lastQueryContext = new RDBQueryContext<IRDBBatchQueryQueryAdded<T>>(this, _queryBuilderContext.CreateChildContext());
            return _lastQueryContext;
        }

        public IRDBBatchQueryQueryAdded<T> AddQueryIf(Func<bool> shouldAddQuery, Action<RDBQueryContext<IRDBBatchQueryQueryAdded<T>>> trueAction)
        {
            if(shouldAddQuery())
            {
                if (_lastQueryContext != null && _lastQueryContext.Query != null)
                    this._queries.Add(_lastQueryContext.Query);
                _lastQueryContext = new RDBQueryContext<IRDBBatchQueryQueryAdded<T>>(this, _queryBuilderContext.CreateChildContext());
                trueAction(_lastQueryContext);
            }
            return this;
        }

        public IRDBBatchQueryQueryAdded<T> Foreach<Q>(IEnumerable<Q> items, Action<Q, IRDBBatchQuery<T>> action)
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

    public interface IRDBBatchQueryQueryAdded<T> : IRDBBatchQuery<T>, IRDBBatchQueryCanAddQuery<T>
    {

    }

    public interface IRDBBatchQueryCanAddQuery<T>
    {
        RDBQueryContext<IRDBBatchQueryQueryAdded<T>> AddQuery();

        IRDBBatchQueryQueryAdded<T> AddQueryIf(Func<bool> shouldAddQuery, Action<RDBQueryContext<IRDBBatchQueryQueryAdded<T>>> trueAction);

        IRDBBatchQueryQueryAdded<T> Foreach<Q>(IEnumerable<Q> items, Action<Q, IRDBBatchQuery<T>> action);
    }
    public interface IRDBBatchQuery<T> : IRDBBatchQueryCanAddQuery<T>, IRDBBatchQueryReadyBatch<T>
    {
        
    }

    public interface IRDBBatchQueryReadyBatch<T>
    {
        T EndBatchQuery();
    }
}
