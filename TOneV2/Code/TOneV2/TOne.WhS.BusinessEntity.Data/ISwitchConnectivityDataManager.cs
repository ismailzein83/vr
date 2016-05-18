using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISwitchConnectivityDataManager : IDataManager
    {
        bool Insert(SwitchConnectivity switchConnectivity, out int insertedId);
        bool Update(SwitchConnectivity switchConnectivity);
        bool AreSwitchConnectivitiesUpdated(ref object updateHandle);
        List<SwitchConnectivity> GetSwitchConnectivities();
    }
}
