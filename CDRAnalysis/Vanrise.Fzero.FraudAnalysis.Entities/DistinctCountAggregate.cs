using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class DistinctCountAggregate : IAggregate
    {

        Func<NormalCDR, bool> _condition;
        MethodInfo _propertyGetMethod;
        Func<NormalCDR, String> _cdrExpressionToCountDistinct;
        HashSet<string> DistinctItems = new HashSet<string>();

        public DistinctCountAggregate(string propertyName, Func<NormalCDR, bool> condition)
        {
            _propertyGetMethod = typeof(NormalCDR).GetProperty(propertyName).GetGetMethod();
            _condition = condition;
        }

        public DistinctCountAggregate(Func<NormalCDR, String> cdrExpressionToCountDistinct, Func<NormalCDR, bool> condition)
        {
            _cdrExpressionToCountDistinct = cdrExpressionToCountDistinct;
            _condition = condition;
        }

        public void Reset()
        {
            DistinctItems.Clear();
        }

        public void EvaluateCDR(NormalCDR cdr)
        {
            if (_condition == null || _condition(cdr))
            {
                if (_cdrExpressionToCountDistinct != null)
                {
                    DistinctItems.Add(_cdrExpressionToCountDistinct(cdr));
                }

                else
                {
                    DistinctItems.Add( (String)_propertyGetMethod.Invoke(cdr, null));
                }
            }
        }

        public decimal GetResult()
        {
            return decimal.Parse(DistinctItems.Count().ToString());
        }


    }
    
}