
namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public interface IAggregate 
    {
        void Reset();
        void EvaluateCDR(NormalCDR normalCDR);
        decimal GetResult();
    }
}
