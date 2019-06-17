using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class GenericBERuntimeSelectorFilter : BERuntimeSelectorFilter
    {
        public bool NotApplicableInSearch { get; set; }

        public GenericBESelectorCondition GenericBESelectorCondition { get; set; }

        public override bool IsMatched(IBERuntimeSelectorFilterSelectorFilterContext context)
        {
            throw new NotImplementedException();
        }
    }



    public class GenericBERecordFilterGroupSelectorCondition : GenericBESelectorCondition
    {
        public override Guid ConfigId { get { return new Guid("FD373089-5799-476E-880C-1F9541259A83"); } }

        public RecordFilterGroup RecordFilter { get; set; }

        public override RecordFilterGroup GetFilterGroup(IGenericBESelectorConditionGetFilterGroupContext context)
        {
            return this.RecordFilter;
        }
    }

    public class GenericBESelectorConditionGroup : GenericBESelectorCondition
    {
        public override Guid ConfigId { get { return new Guid("D8FB248F-168E-484A-B421-6EA4FCF569D2"); } }

        public RecordQueryLogicalOperator Operator { get; set; }
        public List<GenericBESelectorCondition> GenericBESelectorConditionList { get; set; }

        public override RecordFilterGroup GetFilterGroup(IGenericBESelectorConditionGetFilterGroupContext context)
        {
            RecordFilterGroup fitlerGroup = new RecordFilterGroup() { LogicalOperator = Operator, Filters = new List<RecordFilter>() };
            foreach (var condition in GenericBESelectorConditionList)
                fitlerGroup.Filters.Add(condition.GetFilterGroup(context));

            return fitlerGroup;
        }
    }
}