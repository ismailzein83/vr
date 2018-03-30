using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRBulkActionDraftDataManager : BaseSQLDataManager, IVRBulkActionDraftDataManager
    {
    
        #region ctor/Local Variables

        public VRBulkActionDraftDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        const string VRBulkActionDraft_TABLENAME = "VRBulkActionDraftTable";

        #endregion

        #region Public Methods

        public void ClearVRBulkActionDraft(Guid bulkActionDraftIdentifier, DateTime removeBeforeDate)
        {
            ExecuteNonQuerySP("common.sp_VRBulkActionDraft_Clear", bulkActionDraftIdentifier, removeBeforeDate);
        }

        public void CreateVRBulkActionDrafts(IEnumerable<VRBulkActionDraft> vrBulkActionDrafts,Guid bulkActionDraftIdentifier)
        {

            DataTable vrBulkActionDraftsToSave = GetVRBulkActionDraftTable();
            foreach (var vrBulkActionDraft in vrBulkActionDrafts)
            {
                DataRow dr = vrBulkActionDraftsToSave.NewRow();
                FillVRBulkActionDraftRow(dr, vrBulkActionDraft, bulkActionDraftIdentifier);
                vrBulkActionDraftsToSave.Rows.Add(dr);
            }
            vrBulkActionDraftsToSave.EndLoadData();
            if (vrBulkActionDraftsToSave.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[common].[sp_VRBulkActionDraft_Save]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@VRBulkActionDraftTable", SqlDbType.Structured);
                           dtPrm.Value = vrBulkActionDraftsToSave;
                           cmd.Parameters.Add(dtPrm);
                       });
        }

        public IEnumerable<VRBulkActionDraft> GetVRBulkActionDrafts(Guid bulkActionDraftIdentifier)
        {
            return GetItemsSP("common.sp_VRBulkActionDraft_GetByIdentifier", VRBulkActionDraftMapper, bulkActionDraftIdentifier);
        }
        #endregion
      
        #region Private Methods
        private void FillVRBulkActionDraftRow(DataRow dr, VRBulkActionDraft vrBulkActionDraft,Guid bulkActionDraftIdentifier)
        {
            dr["BulkActionDraftIdentifier"] = bulkActionDraftIdentifier;
            dr["ItemId"] = vrBulkActionDraft.ItemId;
        }
        private DataTable GetVRBulkActionDraftTable()
        {
            DataTable dt = new DataTable(VRBulkActionDraft_TABLENAME);
            dt.Columns.Add("BulkActionDraftIdentifier", typeof(Guid));
            dt.Columns.Add("ItemId", typeof(string));
            return dt;
        }
        #endregion

        #region Mappers

        VRBulkActionDraft VRBulkActionDraftMapper(IDataReader reader)
        {
            return new VRBulkActionDraft
            {
                ItemId = reader["ItemId"] as string,
            };
        }

        #endregion
    }
}
