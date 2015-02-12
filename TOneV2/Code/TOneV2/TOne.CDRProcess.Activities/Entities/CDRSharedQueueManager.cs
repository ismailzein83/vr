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

        public TOneQueue<CDRBatch> GetCDRQueue(int switchId)
        {
            string cacheKey = String.Format("CDRQueue_{0}", switchId);
            return GetQueue<CDRBatch>(cacheKey);
        }

        public TOneQueue<CDRBillingBatch> GetCDRQueueForStatistics(int switchId)
        {
            string cacheKey = String.Format("CDRQueueForStatistics_{0}", switchId);
            return GetQueue<CDRBillingBatch>(cacheKey);
        }

        public TOneQueue<CDRBillingBatch> GetCDRQueueForBilling(int switchId)
        {
            string cacheKey = String.Format("CDRQueueForBilling_{0}", switchId);
            return GetQueue<CDRBillingBatch>(cacheKey);
        }
    }
}
