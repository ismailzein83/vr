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

        public List<CustomerZones> GetAllCustomerZones()
        {
            return GetItemsSP("TOneWhS_BE.sp_CustomerZone_GetAll", CustomerZonesMapper);
        }

        public bool AddCustomerZones(CustomerZones customerZones, out int insertedId)
        {
            object customerZonesId;

            string serializedZones = Vanrise.Common.Serializer.Serialize(customerZones.Zones);

            int recordsAffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerZone_Insert", out customerZonesId, customerZones.CustomerId, serializedZones, customerZones.StartEffectiveTime);

            insertedId = (int)customerZonesId;

            return (recordsAffected > 0);
        }

        public bool AreAllCustomerZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_BE].[CustomerZone]", ref updateHandle);
        }

        #region Mappers

        private CustomerZones CustomerZonesMapper(IDataReader reader)
        {
            CustomerZones customerZones = new CustomerZones();

            customerZones.CustomerZonesId = (int)reader["ID"];
            customerZones.CustomerId = (int)reader["CustomerID"];
            customerZones.Zones = Vanrise.Common.Serializer.Deserialize<List<CustomerZone>>(reader["Details"] as string);
            customerZones.StartEffectiveTime = (DateTime)reader["BED"];

            return customerZones;
        }

        #endregion
    }
}
