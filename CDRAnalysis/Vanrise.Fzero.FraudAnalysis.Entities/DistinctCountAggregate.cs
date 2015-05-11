using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class DistinctCountAggregate:IAggregate
    {

        public DistinctCountAggregate(string property, string conditionExpression)
        {

        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void EvaluateCDR(NormalCDR normalCDR)
        {
            throw new NotImplementedException();
        }

        public decimal GetResult()
        {
            throw new NotImplementedException();
        }
    }

    
}