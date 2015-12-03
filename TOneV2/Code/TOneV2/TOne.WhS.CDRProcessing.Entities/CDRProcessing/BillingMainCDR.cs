using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.StatisticManagement;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class BillingMainCDR :IRawItem
    {
        static BillingMainCDR()
        {
            BillingCDRBase BillingCDR = new BillingCDRBase();
            Cost Cost = new Cost();
            Sale Sale = new Sale();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingMainCDR), "BillingCDR", "Cost", "Sale");
        }
        public BillingCDRBase BillingCDR { get; set; }
        public Cost Cost { get; set; }
        public Sale Sale { get; set; }
    }

}
