using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDR.Entities
{
    public class TrafficStatistic : BaseTrafficStatistic
    {
        public int SwitchId { get; set; }

        /// <summary>
        /// Port (Trunk or IP:Port) IN
        /// </summary>
        public string Port_IN { get; set; }

        /// <summary>
        /// Port (Trunk or IP:Port) Out
        /// </summary>
        public string Port_OUT { get; set; }

        public string CustomerId { get; set; }

        public int OurZoneId { get; set; }

        public int OriginatingZoneId { get; set; }

        public string SupplierId { get; set; }

        public int SupplierZoneId { get; set; }

        public DateTime FirstCDRAttempt { get; set; }

        public DateTime LastCDRAttempt { get; set; }

        

        public override string GetGroupKey()
        {
            return GetGroupKey(this.SwitchId, this.Port_IN, this.Port_OUT, this.CustomerId, this.OurZoneId, this.OriginatingZoneId, this.SupplierZoneId);
        }

        public static string GetGroupKey(int switchId, string port_IN, string port_OUT, string customerId, int ourZoneId, int originatingZoneId, int supplierZoneId)
        {
            return String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}", switchId, port_IN, port_OUT, customerId, ourZoneId, originatingZoneId, supplierZoneId);
        }

        public static TrafficStatistic CreateFromKey(int switchId, string port_IN, string port_OUT, string customerId, int ourZoneId, int originatingZoneId, string supplierId, int supplierZoneId)
        {
            return new TrafficStatistic
            {
                SwitchId = switchId,
                Port_IN = port_IN,
                Port_OUT = port_OUT,
                CustomerId = customerId,
                OurZoneId = ourZoneId,
                OriginatingZoneId = originatingZoneId,
                SupplierId = supplierId,
                SupplierZoneId = supplierZoneId
            };
        }
    }
}