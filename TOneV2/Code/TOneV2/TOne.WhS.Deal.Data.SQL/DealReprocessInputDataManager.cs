using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Deal.Data.SQL
{
    public class DealReprocessInputDataManager : BaseSQLDataManager, IDealReprocessInputDataManager
    {
        public void InsertDealReprocessInputs(List<DealReprocessInput> dealReprocessInputs)
        {
            DataTable dtDealProgress = BuildDealReprocessInputTable(dealReprocessInputs);
            ExecuteNonQuerySPCmd("[TOneWhS_Deal].[sp_DealReprocessInput_Insert]", (cmd) =>
            {
                var dtPrm = new SqlParameter("@DealReprocessInputs", SqlDbType.Structured);
                dtPrm.Value = dtDealProgress;
                cmd.Parameters.Add(dtPrm);
            });
        }

        DataTable BuildDealReprocessInputTable(List<DealReprocessInput> dealReprocessInputs)
        {
            DataTable dtDealReprocessInput = GetDealReprocessInputTable();
            dtDealReprocessInput.BeginLoadData();
            foreach (var dealReprocessInput in dealReprocessInputs)
            {
                DataRow dr = dtDealReprocessInput.NewRow();
                dr["DealReprocessInputID"] = dealReprocessInput.DealReprocessInputID;
                dr["DealID"] = dealReprocessInput.DealID;
                dr["ZoneGroupNb"] = dealReprocessInput.ZoneGroupNb;
                dr["IsSale"] = dealReprocessInput.IsSale;
                dr["TierNb"] = dealReprocessInput.TierNb;
                dr["RateTierNb"] = dealReprocessInput.RateTierNb;
                dr["FromTime"] = dealReprocessInput.FromTime;
                dr["ToTime"] = dealReprocessInput.ToTime;
                dr["UpToDurationInSec"] = dealReprocessInput.UpToDurationInSec;
                dtDealReprocessInput.Rows.Add(dr);
            }
            dtDealReprocessInput.EndLoadData();
            return dtDealReprocessInput;
        }

        DataTable GetDealReprocessInputTable()
        {
            DataTable dtDealProgress = new DataTable();
            dtDealProgress.Columns.Add("DealReprocessInputID", typeof(Int64));
            dtDealProgress.Columns.Add("DealID", typeof(Int32));
            dtDealProgress.Columns.Add("ZoneGroupNb", typeof(Int32));
            dtDealProgress.Columns.Add("IsSale", typeof(bool));
            dtDealProgress.Columns.Add("TierNb", typeof(int));
            dtDealProgress.Columns.Add("RateTierNb", typeof(int));
            dtDealProgress.Columns.Add("FromTime", typeof(DateTime));
            dtDealProgress.Columns.Add("ToTime", typeof(DateTime));
            dtDealProgress.Columns.Add("UpToDurationInSec", typeof(decimal));
            return dtDealProgress;
        }
    }

            
}
