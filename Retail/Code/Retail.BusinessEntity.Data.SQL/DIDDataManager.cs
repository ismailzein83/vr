using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Data;
using Vanrise.Data.SQL;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;

namespace  Retail.BusinessEntity.Data.SQL
{
    public class DIDDataManager : BaseSQLDataManager, IDIDDataManager
    {

        #region ctor/Local Variables
        public DIDDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {
        }
        #endregion

        #region Public Methods
        public bool Insert(DID dID, out int insertedId)
        {

            object dIDId;
            string serializedSettings = dID.Settings != null ? Serializer.Serialize(dID.Settings) : null;

            int recordsEffected = ExecuteNonQuerySP("Retail_BE.sp_DID_Insert", out dIDId, dID.Number, serializedSettings);
            insertedId = (recordsEffected > 0) ? (int)dIDId : -1;
            return (recordsEffected > 0);
        }
        public bool Update(DID dID)
        {
            string serializedSettings = dID.Settings != null ? Serializer.Serialize(dID.Settings) : null;

            int recordsEffected = ExecuteNonQuerySP("Retail_BE.sp_DID_Update", dID.DIDId, dID.Number, serializedSettings);
            return (recordsEffected > 0);
        }
        public List<DID> GetAllDIDs()
        {
            return GetItemsSP("Retail_BE.sp_DID_GetAll", DIDMapper);
        }
        public bool AreDIDsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.DID", ref updateHandle);
        }
        #endregion


        #region  Mappers
        private DID DIDMapper(IDataReader reader)
        {
            DID pop = new DID
            {
                DIDId = (int)reader["ID"],
                Number = reader["Number"] as string,
                Settings = Serializer.Deserialize<DIDSettings>(reader["Settings"] as string),

            };
            return pop;
        }
        #endregion

    }
}
