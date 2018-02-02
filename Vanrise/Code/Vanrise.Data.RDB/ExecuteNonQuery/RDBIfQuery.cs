using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBIfQuery<T> : BaseRDBQuery, IRDBIfQuery<T>, IRDBIfQueryReady<T>, IRDBIfQueryConditionDefined<T>, IRDBIfQueryTrueQueryDefined<T>, IRDBIfQueryFalseQueryDefined<T>
    {
        T _parent;
        BaseRDBQueryContext _queryContext;

        public RDBIfQuery(T parent, BaseRDBQueryContext queryContext)
        {
            _parent = parent;
            _queryContext = queryContext;
        }

        public BaseRDBCondition Condition { get; set; }

        public IRDBQueryReady TrueQuery { get; set; }

        public IRDBQueryReady FalseQuery { get; set; }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(" IF ");
            var conditionContext = new RDBConditionToDBQueryContext(context, true);
            queryBuilder.Append(this.Condition.ToDBQuery(conditionContext));
            var trueQueryContext = new RDBQueryGetResolvedQueryContext(context, true);
            queryBuilder.Append(this.TrueQuery.GetResolvedQuery(trueQueryContext).QueryText);
            if(FalseQuery != null)
            {
                queryBuilder.Append(" ELSE ");
                var falseQueryContext = new RDBQueryGetResolvedQueryContext(context, true);
                queryBuilder.Append(this.FalseQuery.GetResolvedQuery(falseQueryContext).QueryText);
            }
            return new RDBResolvedQuery
            {
                QueryText = queryBuilder.ToString()
            };
        }

        public RDBConditionContext<IRDBIfQueryConditionDefined<T>> IfCondition()
            {
                return new RDBConditionContext<IRDBIfQueryConditionDefined<T>>(this, (condition) => this.Condition = condition, null);
            }
        

        public RDBQueryContext<IRDBIfQueryTrueQueryDefined<T>> ThenQuery()
            {
                var queryContext = new RDBQueryContext<IRDBIfQueryTrueQueryDefined<T>>(this, _queryContext);
                this.TrueQuery = queryContext.Query;
                return queryContext;
            }
        

        public RDBQueryContext<IRDBIfQueryFalseQueryDefined<T>> ElseQuery()
            {
                var queryContext = new RDBQueryContext<IRDBIfQueryFalseQueryDefined<T>>(this, _queryContext);
                this.FalseQuery = queryContext.Query;
                return queryContext;
            }
        

        public T EndIf()
        {
            return _parent;
        }
    }

    public interface IRDBIfQueryReady<T> : IRDBQueryReady
    {
        T EndIf();
    }

    public interface IRDBIfQuery<T> : IRDBIfQueryCanDefineCondition<T>
    {

    }

    public interface IRDBIfQueryConditionDefined<T> : IRDBIfQueryCanDefineTrueQuery<T>
    {

    }

    public interface IRDBIfQueryTrueQueryDefined<T> : IRDBIfQueryReady<T>, IRDBIfQueryCanDefineFalseQuery<T>
    {

    }

    public interface IRDBIfQueryFalseQueryDefined<T> : IRDBIfQueryReady<T>
    {

    }

    public interface IRDBIfQueryCanDefineCondition<T>
    {
        RDBConditionContext<IRDBIfQueryConditionDefined<T>> IfCondition();
    }

    public interface IRDBIfQueryCanDefineTrueQuery<T>
    {
        RDBQueryContext<IRDBIfQueryTrueQueryDefined<T>> ThenQuery();
    }

    public interface IRDBIfQueryCanDefineFalseQuery<T>
    {
        RDBQueryContext<IRDBIfQueryFalseQueryDefined<T>> ElseQuery();
    }
}
