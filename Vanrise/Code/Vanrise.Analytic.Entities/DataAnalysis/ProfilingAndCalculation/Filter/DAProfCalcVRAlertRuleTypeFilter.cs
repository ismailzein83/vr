//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Notification.Entities;

//namespace Vanrise.Analytic.Entities
//{
//    class DAProfCalcVRAlertRuleTypeFilter : IVRAlertRuleTypeFilter
//    {
//        public bool IsMatch(VRAlertRuleType alertRuleType)
//        {
//            if (alertRuleType == null)
//                throw new NullReferenceException("alertRuleType");

//            if (alertRuleType.Settings == null)
//                throw new NullReferenceException("alertRuleType.Settings");

//            if (alertRuleType.Settings.ConfigId != DAProfCalcAlertRuleTypeSettings.s_ConfigId)
//                return false;
//            return true;
//        }
//    }
//}
