using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;

namespace Retail.BusinessEntity.Business
{
    public class RetailModuleManager
    {
        AnalyticTableManager analyticTableManager = new AnalyticTableManager();
        public bool IsSMSModuleEnabled(Guid smsBillingStatAnalyticTableId)
        {
            return analyticTableManager.IsAnalyticTableExist(smsBillingStatAnalyticTableId);
        }
        public bool IsVoiceModuleEnabled(Guid voiceBillingStatAnalyticTableId)
        {
            return analyticTableManager.IsAnalyticTableExist(voiceBillingStatAnalyticTableId);
        }
    }
}
