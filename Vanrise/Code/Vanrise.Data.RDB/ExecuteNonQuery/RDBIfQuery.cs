using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBIfQuery<T> : BaseRDBQuery, IRDBIfQuery<T>, IRDBIfQueryReady<T>, IRDBIfQueryConditionDefined<T>, IRDBIfQueryTrueQueryDefined<T>, IRDBIfQueryFalseQueryDefined<T>
    {
        T _parent;
        RDBQueryBuilderContext _queryBuilderContext;

        public RDBIfQuery(T parent, RDBQueryBuilderContext queryBuilderContext)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
        }

        public BaseRDBCondition Condition { get; set; }

        internal string _trueQueryText;

        RDBQueryContext<IRDBIfQueryTrueQueryDefined<T>> _trueQueryContext;
        IRDBQueryReady _trueQuery;
        public IRDBQueryReady TrueQuery
        {
            get
            {
                if (_trueQuery == null)
                {
                    _trueQueryContext.ThrowIfNull("_trueQueryContext");
                    _trueQueryContext.Query.ThrowIfNull("_trueQueryContext.Query");
                    _trueQuery = _trueQueryContext.Query;
                }
                return _trueQuery;
            }
            set
            {
                _trueQuery = value;
            }
        }

        RDBQueryContext<IRDBIfQueryFalseQueryDefined<T>> _falseQueryContext;
        IRDBQueryReady _falseQuery;
        public IRDBQueryReady FalseQuery
        {
            get
            {
                if (_falseQuery == null)
                {
                    if (_falseQueryContext != null)//false query context is not mandatory
                    {
                        _falseQueryContext.Query.ThrowIfNull("_falseQueryContext.Query");
                        _falseQuery = _falseQueryContext.Query;
                    }
                }
                return _falseQuery;
            }
            set
            {
                _falseQuery = value;
            }
        }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(" IF ");            
            var conditionContext = new RDBConditionToDBQueryContext(context, _queryBuilderContext);
            queryBuilder.AppendLine(this.Condition.ToDBQuery(conditionContext));
            queryBuilder.AppendLine(" BEGIN ");
            if (this._trueQueryText != null)
            {
                queryBuilder.AppendLine(this._trueQueryText);
            }
            else
            {
                var trueQueryContext = new RDBQueryGetResolvedQueryContext(context);
                queryBuilder.AppendLine(this.TrueQuery.GetResolvedQuery(trueQueryContext).QueryText);
            }
            queryBuilder.AppendLine(" END ");
            if (FalseQuery != null)
            {
                queryBuilder.AppendLine(" ELSE ");
                queryBuilder.AppendLine(" BEGIN ");
                var falseQueryContext = new RDBQueryGetResolvedQueryContext(context);
                queryBuilder.AppendLine(this.FalseQuery.GetResolvedQuery(falseQueryContext).QueryText);
                queryBuilder.AppendLine(" END ");
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
            _trueQueryContext = new RDBQueryContext<IRDBIfQueryTrueQueryDefined<T>>(this, _queryBuilderContext.CreateChildContext());
            return _trueQueryContext;
        }


        public RDBQueryContext<IRDBIfQueryFalseQueryDefined<T>> ElseQuery()
        {
            _falseQueryContext = new RDBQueryContext<IRDBIfQueryFalseQueryDefined<T>>(this, _queryBuilderContext.CreateChildContext());
            return _falseQueryContext;
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
