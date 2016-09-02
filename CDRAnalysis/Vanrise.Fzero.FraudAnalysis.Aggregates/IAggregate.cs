using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public interface IAggregate 
    {
        void EvaluateCDR(CDR cdr);

        decimal GetResult(INumberProfileParameters strategy);
    }

    public abstract class AggregateState
    {

    }

    public abstract class BaseAggregate
    {
        public abstract AggregateState CreateState();

        public abstract void Evaluate(AggregateState state, CDR cdr);

        public abstract void UpdateExistingFromNew(AggregateState existingState, AggregateState newState);

        public abstract decimal GetResult(AggregateState state, INumberProfileParameters parametersSet);
    }
}
