using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Entities
{
    public class Rule
    {
        public int RuleId { get; set; }

        public int TypeId { get; set; }

        public string RuleDetails { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public string SourceId { get; set; }
    }
}
