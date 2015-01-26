﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Caching;
using TOne.Entities;
using Vanrise.Caching;

namespace TOne.CDRProcess.Activities
{
    public class CDRSharedQueueManager : TOne.Business.SharedQueueManager
    {

        public TOneQueue<List<TABS.CDR>> GetCDRQueue(int switchId)
        {
            string cacheKey = String.Format("CDRQueue_{0}", switchId);
            return GetQueue<List<TABS.CDR>>(cacheKey);
        }

        public TOneQueue<TABS.Billing_CDR_Base> GetCDRQueueForStatistics(int switchId)
        {
            string cacheKey = String.Format("CDRQueueForStatistics_{0}", switchId);
            return GetQueue<TABS.Billing_CDR_Base>(cacheKey);
        }

        public TOneQueue<TABS.Billing_CDR_Base> GetCDRQueueForBilling(int switchId)
        {
            string cacheKey = String.Format("CDRQueueForBilling_{0}", switchId);
            return GetQueue<TABS.Billing_CDR_Base>(cacheKey);
        }
    }
}
