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
        RDBQueryBuilderContext QueryBuilderContext { get; }
    }


    public class RDBConditionToDBQueryContext : BaseRDBResolveQueryContext, IRDBConditionToDBQueryContext
    {
        public RDBConditionToDBQueryContext(IBaseRDBResolveQueryContext parentContext, RDBQueryBuilderContext queryBuilderContext)
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
