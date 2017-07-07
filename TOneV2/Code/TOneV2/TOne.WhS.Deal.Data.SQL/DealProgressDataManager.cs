using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Vanrise.Data.SQL;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Data.SQL
{
    public class DealProgressDataManager : BaseSQLDataManager, IDealProgressDataManager
    {
        #region Constructors

        public DealProgressDataManager()
            : base(GetConnectionStringName("TOneWhS_Analytics_DBConnStringKey", "TOneAnalyticsDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void UpdateDealProgresses(List<DealProgress> dealProgresses)
        {
            DataTable dtDealProgress = BuildDealProgressTable(dealProgresses);
            ExecuteNonQuerySPCmd("[TOneWhS_Deal].[sp_DealProgress_Update]", (cmd) =>
            {
                var dtPrm = new SqlParameter("@DealProgresses", SqlDbType.Structured);
                dtPrm.Value = dtDealProgress;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public void InsertDealProgresses(List<DealProgress> dealProgresses)
        {
            DataTable dtDealProgress = BuildDealProgressTable(dealProgresses);
            ExecuteNonQuerySPCmd("[TOneWhS_Deal].[sp_DealProgress_Insert]", (cmd) =>
            {
                var dtPrm = new SqlParameter("@DealProgresses", SqlDbType.Structured);
                dtPrm.Value = dtDealProgress;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public List<DealProgress> GetDealProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale)
        {
            DataTable dtDealZoneGroup = BuildDealZoneGroupTable(dealZoneGroups);
            return GetItemsSPCmd("[TOneWhS_Deal].[sp_DealProgress_GetByDealZoneGroups]", DealProgressMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@IsSale", isSale));
                var dtPrm = new SqlParameter("@DealZoneGroups", SqlDbType.Structured);
                dtPrm.Value = dtDealZoneGroup;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public void DeleteDealProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale)
        {
            DataTable dtDealZoneGroup = BuildDealZoneGroupTable(dealZoneGroups);
            ExecuteNonQuerySPCmd("[TOneWhS_Deal].[sp_DealProgress_Delete]", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@IsSale", isSale));
                var dtPrm = new SqlParameter("@DealZoneGroups", SqlDbType.Structured);
                dtPrm.Value = dtDealZoneGroup;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public IEnumerable<DealZoneGroup> GetAffectedDealZoneGroups(bool isSale)
        {
            return GetItemsSP("[TOneWhS_Deal].[sp_AffectedDealZoneGroupsToDelete_Get]", DealZoneGroupMapper, isSale);
        }

        public void InsertAffectedDealZoneGroups(HashSet<DealZoneGroup> dealZoneGroups, bool isSale)
        {
            DataTable dtDealZoneGroup = BuildDealZoneGroupTable(dealZoneGroups);
            ExecuteNonQuerySPCmd("[TOneWhS_Deal].[sp_AffectedDealZoneGroupsToDelete_Insert]", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@IsSale", isSale));
                var dtPrm = new SqlParameter("@DealZoneGroups", SqlDbType.Structured);
                dtPrm.Value = dtDealZoneGroup;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public void DeleteAffectedDealZoneGroups()
        {
            ExecuteNonQuerySP("[TOneWhS_Deal].[sp_AffectedDealZoneGroupsToDelete_Delete]");
        }

        #endregion

        #region Private Methods

        DataTable BuildDealProgressTable(List<DealProgress> dealProgresses)
        {
            DataTable dtDealProgress = GetDealProgressTable();
            dtDealProgress.BeginLoadData();
            foreach (var dealProgress in dealProgresses)
            {
                DataRow dr = dtDealProgress.NewRow();
                dr["DealProgressId"] = dealProgress.DealProgressId;
                dr["DealId"] = dealProgress.DealId;
                dr["ZoneGroupNb"] = dealProgress.ZoneGroupNb;
                dr["IsSale"] = dealProgress.IsSale;
                dr["CurrentTierNb"] = dealProgress.CurrentTierNb;

                if (dealProgress.ReachedDurationInSeconds.HasValue)
                    dr["ReachedDurationInSec"] = dealProgress.ReachedDurationInSeconds.Value;
                else
                    dr["ReachedDurationInSec"] = DBNull.Value;

                if (dealProgress.TargetDurationInSeconds.HasValue)
                    dr["TargetDurationInSec"] = dealProgress.TargetDurationInSeconds.Value;
                else
                    dr["TargetDurationInSec"] = DBNull.Value;

                dtDealProgress.Rows.Add(dr);
            }
            dtDealProgress.EndLoadData();
            return dtDealProgress;
        }

        DataTable GetDealProgressTable()
        {
            DataTable dtDealProgress = new DataTable();
            dtDealProgress.Columns.Add("DealProgressId", typeof(Int64));
            dtDealProgress.Columns.Add("DealId", typeof(Int32));
            dtDealProgress.Columns.Add("ZoneGroupNb", typeof(Int32));
            dtDealProgress.Columns.Add("IsSale", typeof(bool));
            dtDealProgress.Columns.Add("CurrentTierNb", typeof(int));
            dtDealProgress.Columns.Add("ReachedDurationInSec", typeof(decimal));
            dtDealProgress.Columns.Add("TargetDurationInSec", typeof(decimal));
            return dtDealProgress;
        }

        DataTable BuildDealZoneGroupTable(HashSet<DealZoneGroup> dealZoneGroups)
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

        private DealProgress DealProgressMapper(IDataReader reader)
        {
            DealProgress dealProgress = new DealProgress
            {
                DealProgressId = (long)reader["ID"],
                DealId = (int)reader["DealId"],
                ZoneGroupNb = (int)reader["ZoneGroupNb"],
                CurrentTierNb = (int)reader["CurrentTierNb"],
                IsSale = (bool)reader["IsSale"],
                ReachedDurationInSeconds = GetReaderValue<decimal?>(reader, "ReachedDurationInSec"),
                TargetDurationInSeconds = GetReaderValue<decimal?>(reader, "TargetDurationInSec"),
                CreatedTime = (DateTime)reader["CreatedTime"]
            };
            return dealProgress;
        }

        private DealZoneGroup DealZoneGroupMapper(IDataReader reader)
        {
            DealZoneGroup dealZoneGroup = new DealZoneGroup
            {
                DealId = (int)reader["DealId"],
                ZoneGroupNb = (int)reader["ZoneGroupNb"],
            };
            return dealZoneGroup;
        }

        #endregion
    }
}
