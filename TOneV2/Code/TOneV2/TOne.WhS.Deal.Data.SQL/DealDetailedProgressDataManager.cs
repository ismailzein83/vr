using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        public List<DealDetailedProgress> GetDealsDetailedProgress(List<int> dealIds, DateTime fromDate, DateTime toDate)
        {
            string dealIdsParameter = null;
            if (dealIds != null && dealIds.Any())
                dealIdsParameter = string.Join(",", dealIds);

            return GetItemsSP("[TOneWhS_Deal].[sp_DealDetailedProgress_GetByDealId]", DealDetailedProgressMapper, dealIdsParameter, fromDate, toDate);
        }

        public List<DealDetailedProgress> GetDealDetailedProgresses(HashSet<DealZoneGroup> dealZoneGroups, bool isSale, DateTime? beginDate, DateTime? endDate)
        {
            DataTable dtDealZoneGroup = BuildDealZoneGroupTable(dealZoneGroups);
            return GetItemsSPCmd("[TOneWhS_Deal].[sp_DealDetailedProgress_GetByDealZoneGroups]", DealDetailedProgressMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@IsSale", isSale));

                SqlParameter beginDateParameter = new SqlParameter() { ParameterName = "@BeginDate" };
                if (beginDate.HasValue)
                    beginDateParameter.Value = beginDate.Value;
                else
                    beginDateParameter.Value = DBNull.Value;
                cmd.Parameters.Add(beginDateParameter);

                SqlParameter endDateParameter = new SqlParameter() { ParameterName = "@EndDate" };
                if (endDate.HasValue)
                    endDateParameter.Value = endDate.Value;
                else
                    endDateParameter.Value = DBNull.Value;
                cmd.Parameters.Add(endDateParameter);

                var dtPrm = new SqlParameter("@DealZoneGroups", SqlDbType.Structured);
                dtPrm.Value = dtDealZoneGroup;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public List<DealDetailedProgress> GetDealDetailedProgressesByDate(bool isSale, DateTime? beginDate, DateTime? endDate)
        {
            return GetItemsSP("[TOneWhS_Deal].[sp_DealDetailedProgress_GetByDate]", DealDetailedProgressMapper, isSale, beginDate, endDate);
        }

        public void InsertDealDetailedProgresses(List<DealDetailedProgress> dealDetailedProgresses)
        {
            DataTable dtDealDetailedProgress = BuildDealDetailedProgressTable(dealDetailedProgresses);
            ExecuteNonQuerySPCmd("[TOneWhS_Deal].[sp_DealDetailedProgress_Insert]", (cmd) =>
            {
                var dtPrm = new SqlParameter("@DealDetailedProgresses", SqlDbType.Structured);
                dtPrm.Value = dtDealDetailedProgress;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public void UpdateDealDetailedProgresses(List<DealDetailedProgress> dealDetailedProgresses)
        {
            DataTable dtDealDetailedProgress = BuildDealDetailedProgressTable(dealDetailedProgresses);
            ExecuteNonQuerySPCmd("[TOneWhS_Deal].[sp_DealDetailedProgress_Update]", (cmd) =>
            {
                var dtPrm = new SqlParameter("@DealDetailedProgresses", SqlDbType.Structured);
                dtPrm.Value = dtDealDetailedProgress;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public void DeleteDealDetailedProgresses(List<long> dealDetailedProgressIds)
        {
            ExecuteNonQuerySP("[TOneWhS_Deal].[sp_DealDetailedProgress_Delete]", string.Join(",", dealDetailedProgressIds));
        }

        public DateTime? GetDealEvaluatorBeginDate(byte[] lastTimestamp)
        {
            object beginDateAsObj = ExecuteScalarSP("[TOneWhS_Deal].[sp_DealDetailedProgress_GetDealEvaluatorBeginDate]", lastTimestamp);

            DateTime? beginDate = null;
            if (beginDateAsObj != DBNull.Value)
                beginDate = (DateTime)beginDateAsObj;

            return beginDate;
        }

        public Byte[] GetMaxTimestamp()
        {
            string query = String.Format("SELECT MAX(timestamp) FROM [TOneWhS_Deal].[DealDetailedProgress] WITH(NOLOCK)");
            return (Byte[])ExecuteScalarText(query, null);
        }

        public void DeleteDealDetailedProgresses(bool isSale, DateTime? beginDate, DateTime? endDate)
        {
            ExecuteNonQuerySP("[TOneWhS_Deal].[sp_DealDetailedProgress_DeleteByDate]", beginDate, endDate, isSale);
        }

        #endregion

        #region Private Methods

        DataTable BuildDealDetailedProgressTable(List<DealDetailedProgress> dealDetailedProgresses)
        {
            DataTable dtDealDetailedProgress = GetDealDetailedProgressTable();
            dtDealDetailedProgress.BeginLoadData();
            foreach (var dealDetailedProgress in dealDetailedProgresses)
            {
                DataRow dr = dtDealDetailedProgress.NewRow();
                dr["DealDetailedProgressId"] = dealDetailedProgress.DealDetailedProgressId;
                dr["DealId"] = dealDetailedProgress.DealId;
                dr["ZoneGroupNb"] = dealDetailedProgress.ZoneGroupNb;
                dr["IsSale"] = dealDetailedProgress.IsSale;

                if (dealDetailedProgress.TierNb.HasValue)
                    dr["TierNb"] = dealDetailedProgress.TierNb.Value;
                else
                    dr["TierNb"] = DBNull.Value;

                if (dealDetailedProgress.RateTierNb.HasValue)
                    dr["RateTierNb"] = dealDetailedProgress.RateTierNb.Value;
                else
                    dr["RateTierNb"] = DBNull.Value;

                dr["ReachedDurationInSec"] = dealDetailedProgress.ReachedDurationInSeconds;
                dr["FromTime"] = dealDetailedProgress.FromTime;
                dr["ToTime"] = dealDetailedProgress.ToTime;
                dtDealDetailedProgress.Rows.Add(dr);
            }
            dtDealDetailedProgress.EndLoadData();
            return dtDealDetailedProgress;
        }

        DataTable GetDealDetailedProgressTable()
        {
            DataTable dtDealProgress = new DataTable();
            dtDealProgress.Columns.Add("DealDetailedProgressId", typeof(Int64));
            dtDealProgress.Columns.Add("DealId", typeof(Int32));
            dtDealProgress.Columns.Add("ZoneGroupNb", typeof(Int32));
            dtDealProgress.Columns.Add("IsSale", typeof(bool));
            dtDealProgress.Columns.Add("TierNb", typeof(int));
            dtDealProgress.Columns.Add("RateTierNb", typeof(int));
            dtDealProgress.Columns.Add("ReachedDurationInSec", typeof(decimal));
            dtDealProgress.Columns.Add("FromTime", typeof(DateTime));
            dtDealProgress.Columns.Add("ToTime", typeof(DateTime));
            return dtDealProgress;
        }

        DataTable BuildDealZoneGroupTable(IEnumerable<DealZoneGroup> dealZoneGroups)
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
                DealDetailedProgressId = (long)reader["ID"],
                DealId = (int)reader["DealId"],
                ZoneGroupNb = (int)reader["ZoneGroupNb"],
                IsSale = (bool)reader["IsSale"],
                TierNb = GetReaderValue<int?>(reader, "TierNb"),
                RateTierNb = GetReaderValue<int?>(reader, "RateTierNb"),
                FromTime = (DateTime)reader["FromTime"],
                ToTime = (DateTime)reader["ToTime"],
                ReachedDurationInSeconds = (decimal)reader["ReachedDurationInSec"],
                CreatedTime = (DateTime)reader["CreatedTime"]
            };
            return dealDetailedProgress;
        }

        #endregion
    }
}