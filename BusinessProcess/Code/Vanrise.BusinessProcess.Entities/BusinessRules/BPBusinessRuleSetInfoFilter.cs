using System;
namespace Vanrise.BusinessProcess.Entities
{
    public class BPBusinessRuleSetInfoFilter
    {
        public Guid? BPDefinitionId { get; set; }

        public int? CanBeParentOfRuleSetId { get; set; }
    }
}
