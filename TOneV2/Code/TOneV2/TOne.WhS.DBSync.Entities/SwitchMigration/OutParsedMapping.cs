using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace TOne.WhS.DBSync.Entities
{
    public class OutParsedMapping
    {
        public string SupplierId { get; set; }
        public StaticValues OutTrunk { get; set; }
        public StaticValues OutCarrier { get; set; }
        public StaticValues OutPrefix { get; set; }
    }
}
