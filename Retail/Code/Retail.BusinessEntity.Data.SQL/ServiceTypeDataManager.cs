using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class ServiceTypeDataManager : BaseSQLDataManager, IServiceTypeDataManager
    {
           
        #region Constructors

        public ServiceTypeDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<ServiceType> GetServiceTypes()
        {
            return GetItemsSP("Retail.sp_ServiceType_GetAll", ServiceTypeMapper);
        }

        public bool Update(ServiceType serviceType)
        {
            string serializedSettings = serviceType.Settings != null ? Vanrise.Common.Serializer.Serialize(serviceType.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_ServiceType_Update", serviceType.ServiceTypeId, serviceType.Name,  serializedSettings);
            return (affectedRecords > 0);
        }

        public bool AreServiceTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail.ServiceType", ref updateHandle);
        }

        #endregion

        #region Mappers

        private ServiceType ServiceTypeMapper(IDataReader reader)
        {
            return new ServiceType()
            {
                ServiceTypeId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<ServiceTypeSettings>(reader["Settings"] as string),
            };
        }

        #endregion
    }
}
