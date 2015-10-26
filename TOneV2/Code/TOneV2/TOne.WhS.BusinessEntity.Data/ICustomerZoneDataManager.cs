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
        List<CustomerZones> GetAllCustomerZones();

        bool AddCustomerZones(CustomerZones customerZones, out int insertedId);

        bool AreAllCustomerZonesUpdated(ref object updateHandle);
    }
}
