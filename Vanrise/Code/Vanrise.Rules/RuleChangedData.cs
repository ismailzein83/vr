using System;
using Vanrise.Rules.Entities;

namespace Vanrise.Rules
{
    public class RuleChangedData<T> where T : BaseRule
    {
        public int RuleChangedId { get; set; }

        public int RuleId { get; set; }

        public int RuleTypeId { get; set; }

        public ActionType ActionType { get; set; }

        public T InitialRule { get; set; }

        public AdditionalInformation AdditionalInformation { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}