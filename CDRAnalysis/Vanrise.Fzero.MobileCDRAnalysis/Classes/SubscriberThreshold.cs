﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class SubscriberThreshold
    {

        public static List<SubscriberThreshold> GetList(int strategyId, string SubscriberNumber)
        {
            //if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(Description))
            //    return GetAll();

            List<SubscriberThreshold> subscriberThresholds = new List<SubscriberThreshold>();
            try
            {
                using (Entities context = new Entities())
                {
                    subscriberThresholds = context.SubscriberThresholds
                        .Include(s => s.Suspicion_Level)
                        .Where(s =>
                            (s.StrategyId == strategyId && s.SubscriberNumber == SubscriberNumber && s.SuspicionLevelId != 1)


                        )
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SubscriberThreshold.GetList()", err);
            }
            return subscriberThresholds;
        }

    }
}
