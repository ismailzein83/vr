﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CDRMainBatch : PersistentQueueItem
    {
        static CDRMainBatch()
        {
            BillingMainCDR cdr = new BillingMainCDR();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDRMainBatch), "MainCDRs");
        }
        public List<BillingMainCDR> MainCDRs { get; set; }
        public override string GenerateDescription()
        {
            return String.Format("CDRMainBatch of {0} CDRs", MainCDRs.Count());
        }
    }
}
