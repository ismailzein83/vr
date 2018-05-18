using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBExpression
    {
        public abstract string ToDBQuery(IRDBExpressionToDBQueryContext context);
    }

    public interface IRDBExpressionToDBQueryContext : IBaseRDBResolveQueryContext
    {
        RDBQueryBuilderContext QueryBuilderContext { get; }
    }

    public class RDBExpressionToDBQueryContext : BaseRDBResolveQueryContext, IRDBExpressionToDBQueryContext
    {
        public RDBExpressionToDBQueryContext(IBaseRDBResolveQueryContext parentContext, RDBQueryBuilderContext queryBuilderContext)
            : base(parentContext)
        {
            this.QueryBuilderContext = queryBuilderContext;
        }

        public RDBQueryBuilderContext QueryBuilderContext
        {
            get;
            private set;
        }
    }
}
