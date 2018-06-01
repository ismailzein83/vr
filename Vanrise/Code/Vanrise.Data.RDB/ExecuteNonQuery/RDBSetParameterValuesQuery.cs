using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSetParameterValuesQuery<T> : BaseRDBQuery, IRDBSetParameterValuesQuery<T>, IRDBSetParameterValuesQueryReady<T>
    { 
        T _parent;
        RDBQueryBuilderContext _queryBuilderContext;

        public RDBSetParameterValuesQuery(T parent, RDBQueryBuilderContext queryBuilderContext)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
        }

        Dictionary<string, BaseRDBExpression> _parameterValues = new Dictionary<string, BaseRDBExpression>();
        
        public IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, BaseRDBExpression value)
        {
            _parameterValues.Add(parameterName, value);
            return this;
        }

        public IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, string value)
        {
            return ParamValue(parameterName, new RDBFixedTextExpression { Value = value });
        }

        public IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, int value)
        {
            return ParamValue(parameterName, new RDBFixedIntExpression { Value = value });
        }

        public IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, long value)
        {
            return ParamValue(parameterName, new RDBFixedLongExpression { Value = value });
        }

        public IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, Decimal value)
        {
            return ParamValue(parameterName, new RDBFixedDecimalExpression { Value = value });
        }

        public IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, float value)
        {
            return ParamValue(parameterName, new RDBFixedFloatExpression { Value = value });
        }

        public IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, DateTime value)
        {
            return ParamValue(parameterName, new RDBFixedDateTimeExpression { Value = value });
        }

        public IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, bool value)
        {
            return ParamValue(parameterName, new RDBFixedBooleanExpression { Value = value });
        }

        public IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, Guid value)
        {
            return ParamValue(parameterName, new RDBFixedGuidExpression { Value = value });
        }

        public T EndParameterValues()
        {
            return _parent;
        }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
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

    public interface IRDBSetParameterValuesQuery<T> : IRDBSetParameterValuesQueryCanAddParameter<T>
    {

    }

    public interface IRDBSetParameterValuesQueryReady<T> : IRDBSetParameterValuesQueryCanAddParameter<T>
    {
        T EndParameterValues();
    }

    public interface IRDBSetParameterValuesQueryCanAddParameter<T>
    {
        IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, BaseRDBExpression value);

        IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, string value);

        IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, int value);

        IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, long value);

        IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, Decimal value);

        IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, float value);

        IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, DateTime value);

        IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, bool value);

        IRDBSetParameterValuesQueryReady<T> ParamValue(string parameterName, Guid value);
    }
}
