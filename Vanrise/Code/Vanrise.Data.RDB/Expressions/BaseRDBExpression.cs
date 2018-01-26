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

    public interface IRDBExpressionToDBQueryContext
    {
        RDBDataProviderType DataProviderType { get; }

        string GetTableAlias(IRDBTableQuerySource table);

        void AddParameterValue(string parameterName, Object value);

        string GenerateParameterName();

        IRDBConditionToDBQueryContext ConditionContext { get; }
    }
}
