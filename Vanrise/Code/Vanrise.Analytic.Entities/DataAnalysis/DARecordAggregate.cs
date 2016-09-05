using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class DARecordAggregate
    {
        public int ConfigId { get; set; }

        public virtual DataRecordFieldType FieldType { get; set; }

        public abstract DARecordAggregateState CreateState(IDARecordAggregateCreateStateContext context);

        public abstract void Evaluate(IDARecordAggregateEvaluationContext context);

        public abstract void UpdateExistingFromNew(IDARecordAggregateUpdateExistingFromNewContext context);

        public abstract dynamic GetResult(IDARecordAggregateGetResultContext context);
    }

    public abstract class DARecordAggregateState
    {

    }

    public interface IDARecordAggregateCreateStateContext
    {
    }

    public interface IDARecordAggregateEvaluationContext
    {
        int DataRecordTypeId { get; }

        dynamic Record { get; }

        DARecordAggregateState State { get; }
    }
    
    public interface IDARecordAggregateUpdateExistingFromNewContext
    {
        DARecordAggregateState ExistingState { get; }

        DARecordAggregateState NewState { get; }
    }

    public interface IDARecordAggregateGetResultContext
    {
        DARecordAggregateState State { get; }
    }
}
