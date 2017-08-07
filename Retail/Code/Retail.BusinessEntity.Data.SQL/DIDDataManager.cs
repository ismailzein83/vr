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
using System.Data.SqlClient;

namespace Retail.BusinessEntity.Data.SQL
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

        public List<DID> GetAllDIDs()
        {
            return GetItemsSP("Retail_BE.sp_DID_GetAll", DIDMapper);
        }

        public bool Insert(DID did, out int insertedId)
        {
            object dIDId;
            string serializedSettings = did.Settings != null ? Serializer.Serialize(did.Settings) : null;

            int recordsEffected = ExecuteNonQuerySP("Retail_BE.sp_DID_Insert", out dIDId, serializedSettings, did.SourceId);
            insertedId = (recordsEffected > 0) ? (int)dIDId : -1;
            return (recordsEffected > 0);
        }

        public bool Insert(List<DID> dids)
        {
            DataTable dtDID = BuildDIDTable(dids);
            int recordsEffected = ExecuteNonQuerySPCmd("[Retail_BE].[sp_DID_InsertMultiple]", (cmd) =>
            {
                var dtPrm = new SqlParameter("@DIDs", SqlDbType.Structured);
                dtPrm.Value = dtDID;
                cmd.Parameters.Add(dtPrm);
            });
            return (recordsEffected > 0);
        }

        public bool Update(DID dID)
        {
            string serializedSettings = dID.Settings != null ? Serializer.Serialize(dID.Settings) : null;

            int recordsEffected = ExecuteNonQuerySP("Retail_BE.sp_DID_Update", dID.DIDId, serializedSettings);
            return (recordsEffected > 0);
        }

        public bool AreDIDsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.DID", ref updateHandle);
        }

        #endregion

        #region Private Methods

        DataTable BuildDIDTable(List<DID> dids)
        {
            DataTable dtDID = GetDIDTable();
            dtDID.BeginLoadData();
            foreach (var did in dids)
            {
                DataRow dr = dtDID.NewRow();
                dr["Settings"] = did.Settings != null ? Serializer.Serialize(did.Settings) : null;
                dr["SourceId"] = did.SourceId;
                dtDID.Rows.Add(dr);
            }
            dtDID.EndLoadData();
            return dtDID;
        }

        DataTable GetDIDTable() 
        {
            DataTable dtDID = new DataTable();
            dtDID.Columns.Add("Settings", typeof(string));
            dtDID.Columns.Add("SourceId", typeof(string));
            return dtDID;
        }

        #endregion

        #region  Mappers

        private DID DIDMapper(IDataReader reader)
        {
            DID pop = new DID
            {
                DIDId = (int)reader["ID"],
                Settings = Serializer.Deserialize<DIDSettings>(reader["Settings"] as string),
                SourceId = reader["SourceID"] as string
            };
            return pop;
        }

        #endregion
    }
}
