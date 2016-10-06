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

        public bool Update(Guid serviceTypeId, string title, ServiceTypeSettings serviceTypeSettings)
        {
            string serializedSettings = serviceTypeSettings != null ? Vanrise.Common.Serializer.Serialize(serviceTypeSettings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_ServiceType_Update", serviceTypeId, title, serializedSettings);
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
                ServiceTypeId = GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<ServiceTypeSettings>(reader["Settings"] as string),
            };
        }

        #endregion
    }
}
