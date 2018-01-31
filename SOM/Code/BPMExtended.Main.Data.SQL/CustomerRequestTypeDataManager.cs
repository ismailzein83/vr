using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using BPMExtended.Main.Common;

namespace BPMExtended.Main.Data.SQL
{
    public class CustomerRequestTypeDataManager : BaseSQLDataManager, ICustomerRequestTypeDataManager
    {
        #region ctor/Local Variables

        public CustomerRequestTypeDataManager()
            : base(GetConnectionStringName("BPMExtended_DBConnStringKey", "BPMExtendedDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<Entities.CustomerRequestType> GetCustomerRequestTypes()
        {
            string query = @"SELECT [ID]
      ,[Name]
      ,[Settings]
      ,[CreatedTime]
	  FROM [BPMExtended].[CustomerRequestType] WITH (NOLOCK)";
            return GetItemsText(query, CustomerRequestTypeMapper, null);
        }

        public bool AreCustomerRequestTypesUpdated(ref object updateHandle)
        {
            return IsDataUpdated("[BPMExtended].[CustomerRequestType]", ref updateHandle);
        }

        #endregion

        #region Private Methods

        private CustomerRequestType CustomerRequestTypeMapper(IDataReader reader)
        {
            var customerRequestType = new CustomerRequestType
            {
                CustomerRequestTypeId = (Guid)reader["ID"],
                Name = reader["Name"] as string
            };
            string serializedSettings = reader["Settings"] as string;
            if (serializedSettings != null)
                customerRequestType.Settings = JSONSerializer.Deserialize<CustomerRequestTypeSettings>(serializedSettings);
            return customerRequestType;
        }

        #endregion
    }
}