using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class SMSCost
    {
        public int MobileNetworkID { get; set; }

        public List<SMSCostOption> CostOptions { get; set; }

    }

    public class SMSCostOption
    {
        public int SupplierId { get; set; }

        public decimal SupplierRate { get; set; }
    }
}