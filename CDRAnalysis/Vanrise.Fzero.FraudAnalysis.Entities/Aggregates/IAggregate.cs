
namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public interface IAggregate 
    {
        void Reset();
        void EvaluateCDR(CDR normalCDR);

        decimal GetResult(Strategy strategy);
    }
}
