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
    }

    public class RDBExpressionToDBQueryContext : BaseRDBResolveQueryContext, IRDBExpressionToDBQueryContext
    {
        public RDBExpressionToDBQueryContext(BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(dataProvider, parameterValues)
        {   
        }

        public RDBExpressionToDBQueryContext(IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {

        }
    }
}
