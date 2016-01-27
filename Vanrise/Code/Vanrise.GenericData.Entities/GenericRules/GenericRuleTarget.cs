using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleTarget : Vanrise.Rules.BaseRuleTarget
    {
        public Dictionary<string, DataRecord> DataRecords { get; set; }
    }
}
