using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class TOneModuleManager
    {
        static Guid voiceBillingStatAnalyticTableId = new Guid("4c1aaa1b-675b-420f-8e60-26b0747ca79b");
        static Guid smsBillingStatAnalyticTableId = new Guid("53e9ebc8-c674-4aff-b6c0-9b3b18f95c1f");
        AnalyticTableManager analyticTableManager = new AnalyticTableManager();
        public bool IsSMSModuleEnabled()
        {
            return analyticTableManager.IsAnalyticTableExist(smsBillingStatAnalyticTableId);
        }
        public bool IsVoiceModuleEnabled()
        {
            return analyticTableManager.IsAnalyticTableExist(voiceBillingStatAnalyticTableId);
        }
    }
}
