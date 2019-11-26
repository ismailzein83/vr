using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public enum GroupingByCarrierType { Customer = 1, Supplier = 2}
    public class RepeatedNumberQuery
    {
        public GroupingByCarrierType? GroupingByCarrierType { get; set; }
        public RepeatedNumberFilter Filter { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int RepeatedMorethan { get; set; }
        public CDRType CDRType { get; set; }
        public string PhoneNumberType { get; set; }
        public string PhoneNumber { get; set; }
    }
}
