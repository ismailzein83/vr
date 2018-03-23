using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data
{
    public interface IVRAlertRuleDataManager : IDataManager 
    {
        List<VRAlertRule> GetVRAlertRules();

        bool AreVRAlertRuleUpdated(ref object updateHandle);

        bool Insert(VRAlertRule VRAlertRuleTypeItem, out long insertedId);

        bool Update(VRAlertRule VRAlertRuleTypeItem);

        bool DisableAlertRule(long vrAlertRuleId);

        bool EnableAlertRule(long vrAlertRuleId);
    }
}
