using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSetParameterValuesQuery : BaseRDBQuery
    { 
        RDBQueryBuilderContext _queryBuilderContext;

        public RDBSetParameterValuesQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        Dictionary<string, BaseRDBExpression> _parameterValues = new Dictionary<string, BaseRDBExpression>();
        
        public void ParamValue(string parameterName, BaseRDBExpression value)
        {
            _parameterValues.Add(parameterName, value);
        }

        public void ParamValue(string parameterName, string value)
        {
             ParamValue(parameterName, new RDBFixedTextExpression { Value = value });
        }

        public void ParamValue(string parameterName, int value)
        {
             ParamValue(parameterName, new RDBFixedIntExpression { Value = value });
        }

        public void ParamValue(string parameterName, long value)
        {
             ParamValue(parameterName, new RDBFixedLongExpression { Value = value });
        }

        public void ParamValue(string parameterName, Decimal value)
        {
             ParamValue(parameterName, new RDBFixedDecimalExpression { Value = value });
        }

        public void ParamValue(string parameterName, float value)
        {
             ParamValue(parameterName, new RDBFixedFloatExpression { Value = value });
        }

        public void ParamValue(string parameterName, DateTime value)
        {
             ParamValue(parameterName, new RDBFixedDateTimeExpression { Value = value });
        }

        public void ParamValue(string parameterName, bool value)
        {
             ParamValue(parameterName, new RDBFixedBooleanExpression { Value = value });
        }

        public void ParamValue(string parameterName, Guid value)
        {
             ParamValue(parameterName, new RDBFixedGuidExpression { Value = value });
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            StringBuilder builder = new StringBuilder();
            if (_parameterValues.Count > 0)
                builder.Append("SELECT ");
            bool isFirstParameter= true;
            foreach(var prmValueEntry in _parameterValues)
            {
                if (!isFirstParameter)
                    builder.Append(",");
                isFirstParameter = false;
                builder.Append(context.GetParameterWithValidate(prmValueEntry.Key).DBParameterName);
                builder.Append(" = ");
                builder.Append(prmValueEntry.Value.ToDBQuery(new RDBExpressionToDBQueryContext(context, _queryBuilderContext)));
            }
            return new RDBResolvedQuery
            {
                QueryText = builder.ToString()
            };
        }
    }
}
