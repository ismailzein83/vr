using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public interface IAggregate 
    {
        void Reset();
        void EvaluateCDR(NormalCDR normalCDR);
        decimal GetResult();
    }
}
