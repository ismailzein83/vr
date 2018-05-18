using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class SetParameterValuesQuery<T> : BaseRDBQuery, ISetParameterValuesQuery<T>, ISetParameterValuesQueryReady<T>
    { 
        T _parent;
        RDBQueryBuilderContext _queryBuilderContext;

        public SetParameterValuesQuery(T parent, RDBQueryBuilderContext queryBuilderContext)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
        }

        Dictionary<string, BaseRDBExpression> _parameterValues = new Dictionary<string, BaseRDBExpression>();
        
        public ISetParameterValuesQueryReady<T> ParamValue(string parameterName, BaseRDBExpression value)
        {
            _parameterValues.Add(parameterName, value);
            return this;
        }

        public ISetParameterValuesQueryReady<T> ParamValue(string parameterName, string value)
        {
            return ParamValue(parameterName, new RDBFixedTextExpression { Value = value });
        }

        public ISetParameterValuesQueryReady<T> ParamValue(string parameterName, int value)
        {
            return ParamValue(parameterName, new RDBFixedIntExpression { Value = value });
        }

        public ISetParameterValuesQueryReady<T> ParamValue(string parameterName, long value)
        {
            return ParamValue(parameterName, new RDBFixedLongExpression { Value = value });
        }

        public ISetParameterValuesQueryReady<T> ParamValue(string parameterName, Decimal value)
        {
            return ParamValue(parameterName, new RDBFixedDecimalExpression { Value = value });
        }

        public ISetParameterValuesQueryReady<T> ParamValue(string parameterName, float value)
        {
            return ParamValue(parameterName, new RDBFixedFloatExpression { Value = value });
        }

        public ISetParameterValuesQueryReady<T> ParamValue(string parameterName, DateTime value)
        {
            return ParamValue(parameterName, new RDBFixedDateTimeExpression { Value = value });
        }

        public ISetParameterValuesQueryReady<T> ParamValue(string parameterName, bool value)
        {
            return ParamValue(parameterName, new RDBFixedBooleanExpression { Value = value });
        }

        public ISetParameterValuesQueryReady<T> ParamValue(string parameterName, Guid value)
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
            foreach(var prmValueEntry in _parameterValues)
            {
                builder.Append(context.GetParameterWithValidate(prmValueEntry.Key));
                builder.Append(" = ");
                builder.Append(prmValueEntry.Value.ToDBQuery(new RDBExpressionToDBQueryContext(context, _queryBuilderContext)));
            }
            return new RDBResolvedQuery
            {
                QueryText = builder.ToString()
            };
        }
    }

    public interface ISetParameterValuesQuery<T> : ISetParameterValuesQueryCanAddParameter<T>
    {

    }

    public interface ISetParameterValuesQueryReady<T> : ISetParameterValuesQueryCanAddParameter<T>
    {
        T EndParameterValues();
    }

    public interface ISetParameterValuesQueryCanAddParameter<T>
    {
        ISetParameterValuesQueryReady<T> ParamValue(string parameterName, BaseRDBExpression value);

        ISetParameterValuesQueryReady<T> ParamValue(string parameterName, string value);

        ISetParameterValuesQueryReady<T> ParamValue(string parameterName, int value);

        ISetParameterValuesQueryReady<T> ParamValue(string parameterName, long value);

        ISetParameterValuesQueryReady<T> ParamValue(string parameterName, Decimal value);

        ISetParameterValuesQueryReady<T> ParamValue(string parameterName, float value);

        ISetParameterValuesQueryReady<T> ParamValue(string parameterName, DateTime value);

        ISetParameterValuesQueryReady<T> ParamValue(string parameterName, bool value);

        ISetParameterValuesQueryReady<T> ParamValue(string parameterName, Guid value);
    }
}
