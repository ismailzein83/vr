using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CustomerZoneDataManager : BaseTOneDataManager, ICustomerZoneDataManager
    {
        public CustomerZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public List<CustomerZones> GetCustomerZones()
        {
            return GetItemsSP("TOneWhS_BE.sp_CustomerZone_GetAll", CustomerZoneMapper);
        }

        public bool AreCustomerZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_BE].[CustomerZone]", ref updateHandle);
        }

        #region Private Members

        private CustomerZones CustomerZoneMapper(IDataReader reader)
        {
            CustomerZones customerZone = new CustomerZones();

            customerZone.CustomerId = (int)reader["CustomerID"];
            customerZone.Zones = Vanrise.Common.Serializer.Deserialize<List<CustomerZone>>(reader["Details"] as string);
            customerZone.StartEffectiveTime = (DateTime)reader["BED"];

            return customerZone;
        }

        #endregion
    }
}
