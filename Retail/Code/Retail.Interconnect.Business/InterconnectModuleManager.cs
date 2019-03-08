//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Analytic.Business;
//using Vanrise.Entities;

//namespace Retail.Interconnect.Business
//{
//    public class InterconnectModuleManager
//    {
//        static Guid voiceBillingStatAnalyticTableId = new Guid("6cd535c0-ac49-46bb-aecf-0eae33823b20");
//        static Guid smsBillingStatAnalyticTableId = new Guid("c1bd3f2f-6213-44d1-9d58-99f81e169930");
//        AnalyticTableManager analyticTableManager = new AnalyticTableManager();
//        public bool IsSMSModuleEnabled()
//        {
//            return analyticTableManager.IsAnalyticTableExist(smsBillingStatAnalyticTableId);
//        }
//        public bool IsVoiceModuleEnabled()
//        {
//            return analyticTableManager.IsAnalyticTableExist(voiceBillingStatAnalyticTableId);
//        }
//    }
//    public class InterconnectModuleUIExtendedSettings : UIExtendedSettings
//    {
//        public override Dictionary<string, object> GetUIParameters()
//        {
//            InterconnectModuleManager interconnectModuleManager = new InterconnectModuleManager();
//            var parameters = new Dictionary<string, object>();
//            parameters.Add("IsVoiceModuleEnabled", interconnectModuleManager.IsVoiceModuleEnabled());
//            parameters.Add("IsSMSModuleEnabled", interconnectModuleManager.IsSMSModuleEnabled());
//            return parameters;
//        }
//    }
//}
