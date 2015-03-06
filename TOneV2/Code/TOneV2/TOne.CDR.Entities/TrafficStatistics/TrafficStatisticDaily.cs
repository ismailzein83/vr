using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDR.Entities
{
    public class TrafficStatisticDaily : BaseTrafficStatistic
    {
        public int SwitchId { get; set; }

        public string CustomerId { get; set; }

        public int OurZoneId { get; set; }

        public int OriginatingZoneId { get; set; }

        public string SupplierId { get; set; }

        public int SupplierZoneId { get; set; }

        public DateTime CallDate { get; set; }

        public override string GetGroupKey()
        {
            return GetGroupKey(this.SwitchId, this.CustomerId, this.OurZoneId, this.OriginatingZoneId, this.SupplierZoneId);
        }

        public static string GetGroupKey(int switchId, string customerId, int ourZoneId, int originatingZoneId, int supplierZoneId)
        {
            return String.Format("{0}^{1}^{2}^{3}^{4}", switchId, customerId, ourZoneId, originatingZoneId, supplierZoneId);
        }
    }
}