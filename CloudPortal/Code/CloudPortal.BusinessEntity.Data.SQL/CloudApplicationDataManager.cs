using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace CloudPortal.BusinessEntity.Data.SQL
{
    public class CloudApplicationDataManager : BaseSQLDataManager, ICloudApplicationDataManager
    {
        public CloudApplicationDataManager()
            : base(GetConnectionStringName("CloudPortal_BE_DBConnStringKey", "CloudPortal_BE_DBConnString"))
        {

        }

        public bool AreApplicationsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("cloud.CloudApplication", ref updateHandle);
        }

        public List<Entities.CloudApplication> GetAllApplications()
        {
            return GetItemsSP("cloud.sp_CloudApplication_GetAll", CloudApplicationMapper);
        }

        #region Mappers

        private CloudApplication CloudApplicationMapper(IDataReader reader)
        {
            CloudApplication app = new CloudApplication
            {
                CloudApplicationId = (int)reader["ID"],
                Name = reader["Name"] as string,
                ApplicationIdentification = Serializer.Deserialize<Vanrise.Security.Entities.CloudApplicationIdentification>(reader["ApplicationIdentification"] as string),
                Settings = Serializer.Deserialize<CloudApplicationSettings>(reader["Settings"] as string)
            };
            return app;
        }

        #endregion
    }
}
