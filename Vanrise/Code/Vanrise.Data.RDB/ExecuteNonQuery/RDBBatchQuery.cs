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
        BaseRDBQueryContext _queryContext;

        public RDBBatchQuery(T parent, BaseRDBQueryContext queryContext)
        {
            _parent = parent;
            _queryContext = queryContext;
        }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            StringBuilder queryBuilder = new StringBuilder();
            foreach (var subQuery in _queries)
            {
                var subQueryContext = new RDBQueryGetResolvedQueryContext(context, true);
                RDBResolvedQuery resolvedSubQuery = subQuery.GetResolvedQuery(subQueryContext);
                queryBuilder.AppendLine(resolvedSubQuery.QueryText);
                queryBuilder.AppendLine();
            }

            return new RDBResolvedQuery
                {
                    QueryText = queryBuilder.ToString()
                };
        }

        //public IRDBBatchQueryReady AddQuery(IRDBQueryReady query)
        //{
        //    this._queries.Add(query);
        //    return this;
        //}

        RDBQueryContext<IRDBBatchQueryReady<T>> _lastQueryContext;
        public RDBQueryContext<IRDBBatchQueryReady<T>> AddQuery()
        {
            if (_lastQueryContext != null)
                this._queries.Add(_lastQueryContext.Query);
            _lastQueryContext = new RDBQueryContext<IRDBBatchQueryReady<T>>(this, _queryContext);
            return _lastQueryContext;
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
    }

    public interface IRDBBatchQueryReady<T>
    {
        RDBQueryContext<IRDBBatchQueryReady<T>> AddQuery();

        T EndBatchQuery();
    }
}
