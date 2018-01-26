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

    public interface IRDBConditionToDBQueryContext
    {
        RDBDataProviderType DataProviderType { get; }

        string GetTableAlias(IRDBTableQuerySource table);

        IRDBExpressionToDBQueryContext ExpressionContext { get; }
    }
}
