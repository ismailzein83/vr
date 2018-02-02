using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBQuery : IRDBQueryReady
    {   
        RDBResolvedQuery IRDBQueryReady.GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            return this.GetResolvedQuery(context);
        }

        protected abstract RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context);
    }

    public interface IRDBQueryGetResolvedQueryContext : IBaseRDBResolveQueryContext
    {
    }

    public class RDBQueryGetResolvedQueryContext : BaseRDBResolveQueryContext, IRDBQueryGetResolvedQueryContext
    {
        public RDBQueryGetResolvedQueryContext(BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(queryContext, dataProvider, parameterValues)
        {
        }

        public RDBQueryGetResolvedQueryContext(IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {

        }
    }

    public interface IRDBQueryReady
    {
        RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context);
    }
}
