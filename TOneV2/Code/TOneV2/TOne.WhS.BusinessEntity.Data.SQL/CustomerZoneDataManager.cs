﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CustomerZoneDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ICustomerZoneDataManager
    {
        #region ctor/Local Variables
        public CustomerZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<CustomerZones> GetAllCustomerZones()
        {
            return GetItemsSP("TOneWhS_BE.sp_CustomerZone_GetAll", CustomerZonesMapper);
        }
        public bool AreAllCustomerZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_BE].[CustomerZone]", ref updateHandle);
        }
        public bool AddCustomerZones(CustomerZones customerZones, out int insertedId)
        {
            object customerZonesId;

            string serializedCountries = Vanrise.Common.Serializer.Serialize(customerZones.Countries);

            int recordsAffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerZone_Insert", out customerZonesId, customerZones.CustomerId, serializedCountries, customerZones.StartEffectiveTime);

            insertedId = (int)customerZonesId;

            return (recordsAffected > 0);
        }
        #endregion  
      
        #region Private Methods
        #endregion

        #region Mappers

        private CustomerZones CustomerZonesMapper(IDataReader reader)
        {
            CustomerZones customerZones = new CustomerZones();

            customerZones.CustomerZonesId = (int)reader["ID"];
            customerZones.CustomerId = (int)reader["CustomerID"];
            customerZones.Countries = Vanrise.Common.Serializer.Deserialize<List<CustomerCountry>>(reader["Details"] as string);
            customerZones.StartEffectiveTime = (DateTime)reader["BED"];

            return customerZones;
        }

        #endregion
    }
}
