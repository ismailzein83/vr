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
    public class DealDetailedProgressDataManager : BaseSQLDataManager, IDealDetailedProgressDataManager
    {
        #region Constructors

        public DealDetailedProgressDataManager()
            : base(GetConnectionStringName("TOneWhS_Analytics_DBConnStringKey", "TOneAnalyticsDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<DealDetailedProgress> GetDealDetailedProgress(List<DealZoneGroup> dealZoneGroups)
        {
            DataTable dtDealZoneGroup = BuildDealZoneGroupTable(dealZoneGroups);
            return GetItemsSPCmd("[TOneWhS_Deal].[sp_DealDetailedProgress_GetByDealZoneGroups]", DealDetailedProgressMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@DealZoneGroups", SqlDbType.Structured);
                dtPrm.Value = dtDealZoneGroup;
                cmd.Parameters.Add(dtPrm);
            });
        }

        DataTable BuildDealZoneGroupTable(List<DealZoneGroup> dealZoneGroups)
        {
            DataTable dtDealZoneGroup = GetDealZoneGroupTable();
            dtDealZoneGroup.BeginLoadData();
            foreach (var dealZoneGroup in dealZoneGroups)
            {
                DataRow dr = dtDealZoneGroup.NewRow();
                dr["DealId"] = dealZoneGroup.DealId;
                dr["ZoneGroupNb"] = dealZoneGroup.ZoneGroupNb;
                dtDealZoneGroup.Rows.Add(dr);
            }
            dtDealZoneGroup.EndLoadData();
            return dtDealZoneGroup;
        }

        DataTable GetDealZoneGroupTable()
        {
            DataTable dtDealZoneGroup = new DataTable();
            dtDealZoneGroup.Columns.Add("DealId", typeof(Int32));
            dtDealZoneGroup.Columns.Add("ZoneGroupNb", typeof(Int32));
            return dtDealZoneGroup;
        }
        #endregion

        #region  Mappers

        private DealDetailedProgress DealDetailedProgressMapper(IDataReader reader)
        {
            DealDetailedProgress dealDetailedProgress = new DealDetailedProgress
            {
                DealDetailedProgressID = (long)reader["ID"],
                DealID = (int)reader["ID"],
                ZoneGroupNb = (int)reader["ZoneGroupNb"],
                IsSale = (bool)reader["IsSale"],
                TierNb = (int)reader["TierNb"],
                RateTierNb = (int)reader["RateTierNb"],
                FromTime = (DateTime)reader["FromTime"],
                ToTime = (DateTime)reader["ToTime"],
                ReachedDurationInSeconds = GetReaderValue<decimal?>(reader, "ReachedDurationInSec"),
                CreatedTime = (DateTime)reader["CreatedTime"]
            };
            return dealDetailedProgress;
        }

        #endregion
    }
}
