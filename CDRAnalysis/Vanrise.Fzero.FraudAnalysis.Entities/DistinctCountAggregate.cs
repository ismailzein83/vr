using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class DistinctCountAggregate : IAggregate
    {

        Func<CDR, bool> _condition;
        MethodInfo _propertyGetMethod;
        Func<CDR, Object> _cdrExpressionToCountDistinct;
        HashSet<Object> _distinctItems = new HashSet<Object>();

       

        public DistinctCountAggregate(Func<CDR, Object> cdrExpressionToCountDistinct, Func<CDR, bool> condition)
        {
            this._cdrExpressionToCountDistinct = cdrExpressionToCountDistinct;
            this._condition = condition;
        }

        public void Reset()
        {
            _distinctItems.Clear();
        }

        public void EvaluateCDR(CDR cdr)
        {
            if (this._condition == null || this._condition(cdr))
            {
                if (this._cdrExpressionToCountDistinct != null)
                {
                    _distinctItems.Add(this._cdrExpressionToCountDistinct(cdr));
                }

                else
                {
                    _distinctItems.Add((String)this._propertyGetMethod.Invoke(cdr, null));
                }
            }
        }

        public decimal GetResult()
        {
            return _distinctItems.Count();
        }


    }
    
}