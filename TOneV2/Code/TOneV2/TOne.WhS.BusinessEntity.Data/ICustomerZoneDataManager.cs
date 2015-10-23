using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ICustomerZoneDataManager: IDataManager
    {
        List<CustomerZones> GetCustomerZones();

        bool AddCustomerZones(CustomerZones customerZones, out int insertedId);

        bool AreCustomerZonesUpdated(ref object updateHandle);
    }
}
