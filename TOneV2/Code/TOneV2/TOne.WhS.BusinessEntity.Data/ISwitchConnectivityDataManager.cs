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
        List<SwitchConnectivity> GetSwitchConnectivities();

        bool AreSwitchConnectivitiesUpdated(ref object updateHandle);

        bool Insert(SwitchConnectivity switchConnectivity, out int insertedId);
        
        bool Update(SwitchConnectivity switchConnectivity);
    }
}
