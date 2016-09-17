using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class DistributorDataManager : BaseSQLDataManager, IDistributorDataManager
    {

        public IEnumerable<Distributor> GetDistributors()
        {
            return GetItemsSP("Retail.sp_Distributor_GetAll", DistributorMapper);
        }

        public bool Insert(Distributor distributor, out long insertedId)
        {
            string serializedSettings = distributor.Settings != null ? Serializer.Serialize(distributor.Settings) : null;
            object distributorId;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Distributor_Insert", out distributorId, distributor.Name, distributor.Type, serializedSettings, distributor.SourceId);

            if (affectedRecords > 0)
            {
                insertedId = (int)distributorId;
                return true;
            }
            insertedId = -1;
            return false;
        }

        public bool Update(Distributor distributor)
        {
            string serializedSettings = distributor.Settings != null ? Serializer.Serialize(distributor.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_Distributor_Update", distributor.Id, distributor.Name, distributor.Type, serializedSettings, distributor.SourceId);
            return (affectedRecords > 0);
        }

        public bool AreDistributorsUpdated(ref object updateHandle)
        {
            return IsDataUpdated("Retail.Distributor", ref updateHandle);
        }

        #region Mapper
        private Distributor DistributorMapper(IDataReader reader)
        {
            return new Distributor
            {
                Id = (long)reader["ID"],
                Name = reader["Name"] as string,
                Type = reader["Type"] as string,
                Settings = Serializer.Deserialize<DistributorSetting>(reader["Settings"] as string),
                SourceId = reader["SourceID"] as string
            };
        }

        #endregion

    }
}
