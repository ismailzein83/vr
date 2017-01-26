using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRApplicationVisibilityDataManager : IDataManager
    {
        List<VRApplicationVisibility> GetVRApplicationVisibilities();

        bool AreVRApplicationVisibilityUpdated(ref object updateHandle);

        bool Insert(VRApplicationVisibility vrApplicationVisibilityItem);

        bool Update(VRApplicationVisibility vrApplicationVisibilityItem);
    }
}
