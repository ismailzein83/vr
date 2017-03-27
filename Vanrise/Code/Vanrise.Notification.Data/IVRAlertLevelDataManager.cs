using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data
{
    public interface IVRAlertLevelDataManager : IDataManager
    {
        List<VRAlertLevel> GetAlertLevel();

        bool AreAlertLevelUpdated(ref object updateHandle);

        bool Insert(VRAlertLevel alertLevelItem);

        bool Update(VRAlertLevel alertLevelItem);
    }
}
