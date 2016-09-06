using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data
{
    public interface IVRAlertRuleTypeDataManager : IDataManager
    {
        List<VRAlertRuleType> GetVRAlertRuleTypes();

        bool AreVRAlertRuleTypeUpdated(ref object updateHandle);

        bool Insert(VRAlertRuleType vrAlertRuleTypeItem);

        bool Update(VRAlertRuleType vrAlertRuleTypeItem);
    }
}
