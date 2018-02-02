using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBCondition
    {
        public abstract string ToDBQuery(IRDBConditionToDBQueryContext context);
    }

    public interface IRDBConditionToDBQueryContext : IBaseRDBResolveQueryContext
    {
    }


    public class RDBConditionToDBQueryContext : BaseRDBResolveQueryContext, IRDBConditionToDBQueryContext
    {
        public RDBConditionToDBQueryContext(BaseRDBQueryContext queryContext, BaseRDBDataProvider dataProvider, Dictionary<string, Object> parameterValues)
            : base(queryContext, dataProvider, parameterValues)
        {   
        }

        public RDBConditionToDBQueryContext(IBaseRDBResolveQueryContext parentContext, bool newQueryScope)
            : base(parentContext, newQueryScope)
        {

        }
    }

}
