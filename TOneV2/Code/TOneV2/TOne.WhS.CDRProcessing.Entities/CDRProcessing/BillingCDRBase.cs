using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class BillingCDRBase
    {
        static BillingCDRBase()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingCDRBase), "ID", "Attempt", "CustomerId", "SupplierId");
        }
        
        public int ID { get; set; }
        public DateTime Attempt { get; set; }
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }

    }
}
