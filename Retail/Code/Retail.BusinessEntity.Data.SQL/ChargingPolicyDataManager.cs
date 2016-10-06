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
    public class ChargingPolicyDataManager : BaseSQLDataManager, IChargingPolicyDataManager
    {
        #region Constructors

        public ChargingPolicyDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<ChargingPolicy> GetChargingPolicies()
        {
            return GetItemsSP("Retail.sp_ChargingPolicy_GetAll", ChargingPolicyMapper);
        }

        public bool Insert(ChargingPolicy chargingPolicy, out int insertedId)
        {
            object chargingPolicyId;
            string serializedSettings = chargingPolicy.Settings != null ? Vanrise.Common.Serializer.Serialize(chargingPolicy.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("Retail.sp_ChargingPolicy_Insert", out chargingPolicyId, chargingPolicy.Name, chargingPolicy.ServiceTypeId, serializedSettings);

            if (affectedRecords > 0)
            {
                insertedId = (int)chargingPolicyId;
                return true;
            }

            insertedId = -1;
            return false;
        }

        public bool Update(ChargingPolicyToEdit chargingPolicy)
        {
            string serializedSettings = chargingPolicy.Settings != null ? Vanrise.Common.Serializer.Serialize(chargingPolicy.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_ChargingPolicy_Update", chargingPolicy.ChargingPolicyId, chargingPolicy.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        public bool AreChargingPoliciesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail.ChargingPolicy", ref updateHandle);
        }

        #endregion

        #region Mappers

        private ChargingPolicy ChargingPolicyMapper(IDataReader reader)
        {
            return new ChargingPolicy()
            {
                ChargingPolicyId = (int)reader["ID"],
                Name = reader["Name"] as string,
                ServiceTypeId = GetReaderValue<Guid>(reader,"ServiceTypeId"),
                Settings = Vanrise.Common.Serializer.Deserialize<ChargingPolicySettings>(reader["Settings"] as string)
            };
        }

        #endregion
    }
}
