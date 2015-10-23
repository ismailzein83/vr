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

        public bool AddCustomerZones(CustomerZones customerZones, out int insertedId)
        {
            object customerZonesId;

            string serializedZones = Vanrise.Common.Serializer.Serialize(customerZones.Zones);

            int recordsAffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerZone_Insert", out customerZonesId, customerZones.CustomerId, serializedZones, customerZones.StartEffectiveTime);

            insertedId = (int)customerZonesId;

            return (recordsAffected > 0);
        }

        public bool AreCustomerZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_BE].[CustomerZone]", ref updateHandle);
        }

        private CustomerZones CustomerZoneMapper(IDataReader reader)
        {
            CustomerZones customerZone = new CustomerZones();

            customerZone.CustomerId = (int)reader["CustomerID"];
            customerZone.Zones = Vanrise.Common.Serializer.Deserialize<List<CustomerZone>>(reader["Details"] as string);
            customerZone.StartEffectiveTime = (DateTime)reader["BED"];

            return customerZone;
        }
    }
}
