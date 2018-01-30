using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Entities
{
    public enum ActionType { AddedRule = 0, UpdatedRule = 1, DeletedRule = 2 }

    public class RuleChanged
    {
        public int RuleChangedId { get; set; }

        public int RuleId { get; set; }

        public int RuleTypeId { get; set; }

        public ActionType ActionType { get; set; }

        public string InitialRule { get; set; }

        public string AdditionalInformation { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
