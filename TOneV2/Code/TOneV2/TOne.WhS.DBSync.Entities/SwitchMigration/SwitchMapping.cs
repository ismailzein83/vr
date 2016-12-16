using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace TOne.WhS.DBSync.Entities
{
    public class SwitchMapping
    {
        public int SwitchId { get; set; }
        public string CarrierId { get; set; }
        public InOutMapping InMapping { get; set; }
        public InOutMapping OutMapping { get; set; }
    }
    public class InOutMapping
    {
        public List<string> InOutCarrier { get; set; }
        public List<string> InOutPrefix { get; set; }
        public List<string> InOutTrunk { get; set; }
    }

}
