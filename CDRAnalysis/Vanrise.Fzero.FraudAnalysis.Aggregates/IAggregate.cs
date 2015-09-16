using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public interface IAggregate 
    {
        void Reset();
        void EvaluateCDR(CDR normalCDR);

        decimal GetResult(INumberProfileParameters strategy);
    }
}
