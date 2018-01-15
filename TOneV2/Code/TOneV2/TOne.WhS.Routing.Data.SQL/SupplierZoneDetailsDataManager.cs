using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.Routing.Entities;
using Vanrise.Caching;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class SupplierZoneDetailsDataManager : RoutingDataManager, ISupplierZoneDetailsDataManager
    {
        readonly string[] columns = { "SupplierId", "SupplierZoneId", "EffectiveRateValue", "SupplierServiceIds", "ExactSupplierServiceIds", "SupplierServiceWeight", "SupplierRateId", "SupplierRateEED", "VersionNumber" };
        public DateTime? EffectiveDate { get; set; }
        public bool? IsFuture { get; set; }


        #region Public Methods

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(SupplierZoneDetail record, object dbApplyStream)
        {
            string supplierServiceIds = record.SupplierServiceIds != null ? string.Join(",", record.SupplierServiceIds) : null;
            string exactSupplierServiceIds = record.ExactSupplierServiceIds != null ? string.Join(",", record.ExactSupplierServiceIds) : null;

            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}", record.SupplierId, record.SupplierZoneId,
                decimal.Round(record.EffectiveRateValue, 8), supplierServiceIds, exactSupplierServiceIds, record.SupplierServiceWeight, record.SupplierRateId,
                record.SupplierRateEED.HasValue ? GetDateTimeForBCP(record.SupplierRateEED) : "", record.VersionNumber);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[SupplierZoneDetail]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public void ApplySupplierZoneDetailsForDB(object preparedSupplierZoneDetails)
        {
            InsertBulkToTable(preparedSupplierZoneDetails as BaseBulkInsertInfo);
        }

        public void SaveSupplierZoneDetailsForDB(List<SupplierZoneDetail> supplierZoneDetails)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SupplierZoneDetail supplierZoneDetail in supplierZoneDetails)
                WriteRecordToStream(supplierZoneDetail, dbApplyStream);
            Object preparedSupplierZoneDetails = FinishDBApplyStream(dbApplyStream);
            ApplySupplierZoneDetailsForDB(preparedSupplierZoneDetails);
        }

        public Dictionary<long, SupplierZoneDetail> GetCachedSupplierZoneDetails()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierZoneDetailsCacheManager>();

            return cacheManager.GetOrCreateObject("SupplierZoneDetails", this.RoutingDatabase.ID, SupplierZoneDetailsCacheExpirationChecker.Instance, () =>
           {
               IEnumerable<SupplierZoneDetail> supplierZoneDetails = GetItemsText(query_GetSupplierZoneDetails, SupplierZoneDetailMapper, null);
               return supplierZoneDetails.ToDictionary(itm => itm.SupplierZoneId, itm => itm);
           });
        }

        public IEnumerable<SupplierZoneDetail> GetSupplierZoneDetails()
        {
            string query = query_GetSupplierZoneDetails.Replace("#FILTER#", string.Empty);
            return GetItemsText(query, SupplierZoneDetailMapper, null);
        }

        public IEnumerable<SupplierZoneDetail> GetFilteredSupplierZoneDetailsBySupplierZone(IEnumerable<long> supplierZoneIds)
        {
            DataTable dtZoneIds = BuildZoneIdsTable(new HashSet<long>(supplierZoneIds));
            return GetItemsText(query_GetFilteredSupplierZoneDetailsBySupplierZones, SupplierZoneDetailMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ZoneList", SqlDbType.Structured);
                dtPrm.TypeName = "LongIDType";
                dtPrm.Value = dtZoneIds;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public List<SupplierZoneDetail> GetSupplierZoneDetailsAfterVersionNumber(int versionNumber)
        {
            string query = query_GetSupplierZoneDetails.Replace("#FILTER#", string.Format("WHERE VersionNumber > {0}", versionNumber));
            return GetItemsText(query, SupplierZoneDetailMapper, null);
        }

        public void UpdateSupplierZoneDetails(List<SupplierZoneDetail> supplierZoneDetails)
        {
            DataTable dtSupplierZoneDetails = BuildSupplierZoneDetailsTable(supplierZoneDetails);
            ExecuteNonQueryText(query_UpdateSupplierZoneDetails.ToString(), (cmd) =>
            {
                var dtPrm = new SqlParameter("@SupplierZoneDetails", SqlDbType.Structured);
                dtPrm.TypeName = "SupplierZoneDetailType";
                dtPrm.Value = dtSupplierZoneDetails;
                cmd.Parameters.Add(dtPrm);
            });
        }

        #endregion 

        #region Private Methods

        private DataTable BuildSupplierZoneDetailsTable(List<SupplierZoneDetail> supplierZoneDetails)
        {
            DataTable dtSupplierZoneDetailsInfo = new DataTable();
            dtSupplierZoneDetailsInfo.Columns.Add("SupplierId", typeof(Int32));
            dtSupplierZoneDetailsInfo.Columns.Add("SupplierZoneId", typeof(Int64));
            dtSupplierZoneDetailsInfo.Columns.Add("EffectiveRateValue", typeof(decimal));
            dtSupplierZoneDetailsInfo.Columns.Add("SupplierServiceIds", typeof(string));
            dtSupplierZoneDetailsInfo.Columns.Add("ExactSupplierServiceIds", typeof(string));
            dtSupplierZoneDetailsInfo.Columns.Add("SupplierServiceWeight", typeof(Int32));
            dtSupplierZoneDetailsInfo.Columns.Add("SupplierRateId", typeof(Int64));
            dtSupplierZoneDetailsInfo.Columns.Add("SupplierRateEED", typeof(DateTime));
            dtSupplierZoneDetailsInfo.Columns.Add("VersionNumber", typeof(Int32));
            dtSupplierZoneDetailsInfo.BeginLoadData();

            foreach (var supplierZoneDetail in supplierZoneDetails)
            {
                string supplierZoneServiceIds = supplierZoneDetail.SupplierServiceIds != null ? string.Join(",", supplierZoneDetail.SupplierServiceIds) : null;
                string exactSupplierZoneServiceIds = supplierZoneDetail.ExactSupplierServiceIds != null ? string.Join(",", supplierZoneDetail.ExactSupplierServiceIds) : null;

                DataRow dr = dtSupplierZoneDetailsInfo.NewRow();
                dr["SupplierId"] = supplierZoneDetail.SupplierId;
                dr["SupplierZoneId"] = supplierZoneDetail.SupplierZoneId;
                dr["EffectiveRateValue"] = supplierZoneDetail.EffectiveRateValue;
                dr["SupplierServiceIds"] = supplierZoneServiceIds;
                dr["ExactSupplierServiceIds"] = exactSupplierZoneServiceIds;
                dr["SupplierServiceWeight"] = supplierZoneDetail.SupplierServiceWeight;
                dr["SupplierRateId"] = supplierZoneDetail.SupplierRateId;

                if (supplierZoneDetail.SupplierRateEED.HasValue)
                    dr["SupplierRateEED"] = supplierZoneDetail.SupplierRateEED.Value;
                else
                    dr["SupplierRateEED"] = DBNull.Value;

                dr["VersionNumber"] = supplierZoneDetail.VersionNumber;
                dtSupplierZoneDetailsInfo.Rows.Add(dr);
            }
            dtSupplierZoneDetailsInfo.EndLoadData();
            return dtSupplierZoneDetailsInfo;
        }

        private SupplierZoneDetail SupplierZoneDetailMapper(IDataReader reader)
        {
            string supplierServiceIds = reader["SupplierServiceIds"] as string;
            string exactSupplierServiceIds = reader["ExactSupplierServiceIds"] as string;

            return new SupplierZoneDetail()
            {
                SupplierId = (int)reader["SupplierId"],
                SupplierZoneId = (long)reader["SupplierZoneId"],
                EffectiveRateValue = GetReaderValue<decimal>(reader, "EffectiveRateValue"),
                SupplierServiceIds = !string.IsNullOrEmpty(supplierServiceIds) ? new HashSet<int>(supplierServiceIds.Split(',').Select(itm => int.Parse(itm))) : null,
                ExactSupplierServiceIds = !string.IsNullOrEmpty(exactSupplierServiceIds) ? new HashSet<int>(exactSupplierServiceIds.Split(',').Select(itm => int.Parse(itm))) : null,
                SupplierServiceWeight = GetReaderValue<int>(reader, "SupplierServiceWeight"),
                SupplierRateId = (long)reader["SupplierRateId"],
                SupplierRateEED = GetReaderValue<DateTime?>(reader, "SupplierRateEED"),
                VersionNumber = (int)reader["VersionNumber"]
            };
        }

        private DataTable BuildZoneIdsTable(HashSet<long> zoneIds)
        {
            DataTable dtZoneInfo = new DataTable();
            dtZoneInfo.Columns.Add("ZoneID", typeof(Int64));
            dtZoneInfo.BeginLoadData();
            foreach (var z in zoneIds)
            {
                DataRow dr = dtZoneInfo.NewRow();
                dr["ZoneID"] = z;
                dtZoneInfo.Rows.Add(dr);
            }
            dtZoneInfo.EndLoadData();
            return dtZoneInfo;
        }

        #endregion

        #region Private Classes

        private class SupplierZoneDetailsCacheManager : BaseCacheManager<int>
        {

        }

        private class SupplierZoneDetailsCacheExpirationChecker : CacheExpirationChecker
        {
            static SupplierZoneDetailsCacheExpirationChecker s_instance = new SupplierZoneDetailsCacheExpirationChecker();
            public static SupplierZoneDetailsCacheExpirationChecker Instance
            {
                get
                {
                    return s_instance;
                }
            }

            public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
            {
                TimeSpan entitiesTimeSpan = TimeSpan.FromMinutes(5);
                SlidingWindowCacheExpirationChecker slidingWindowCacheExpirationChecker = new SlidingWindowCacheExpirationChecker(entitiesTimeSpan);
                return slidingWindowCacheExpirationChecker.IsCacheExpired(context);
            }
        }

        #endregion

        #region Queries

        const string query_GetSupplierZoneDetails = @"                                                       
                                           SELECT  zd.[SupplierId]
                                                  ,zd.[SupplierZoneId]
                                                  ,zd.[EffectiveRateValue]
                                                  ,zd.[SupplierServiceIds]
                                                  ,zd.[ExactSupplierServiceIds]
                                                  ,zd.[SupplierServiceWeight]
                                                  ,zd.[SupplierRateId]
                                                  ,zd.[SupplierRateEED]
                                                  ,zd.[VersionNumber]
                                           FROM [dbo].[SupplierZoneDetail] zd with(nolock)";

        const string query_GetFilteredSupplierZoneDetailsBySupplierZones = @"                                                       
                                           SELECT  zd.[SupplierId]
                                                  ,zd.[SupplierZoneId]
                                                  ,zd.[EffectiveRateValue]
                                                  ,zd.[SupplierServiceIds]
                                                  ,zd.[ExactSupplierServiceIds]
                                                  ,zd.[SupplierServiceWeight]
                                                  ,zd.[SupplierRateId]
                                                  ,zd.[SupplierRateEED]
                                                  ,zd.[VersionNumber]
                                           FROM [dbo].[SupplierZoneDetail] zd with(nolock)
                                           JOIN @ZoneList z ON z.ID = zd.SupplierZoneId";

        const string query_UpdateSupplierZoneDetails = @"
                                            Update  supplierZoneDetails set 
                                                    supplierZoneDetails.EffectiveRateValue = szd.EffectiveRateValue,
                                                    supplierZoneDetails.SupplierServiceIds = szd.SupplierServiceIds,
                                                    supplierZoneDetails.ExactSupplierServiceIds = szd.ExactSupplierServiceIds,
                                                    supplierZoneDetails.SupplierServiceWeight = szd.SupplierServiceWeight,
                                                    supplierZoneDetails.SupplierRateId = szd.SupplierRateId,
                                                    supplierZoneDetails.SupplierRateEED = szd.SupplierRateEED,
                                                    supplierZoneDetails.VersionNumber = szd.VersionNumber
                                            FROM [dbo].[SupplierZoneDetail] supplierZoneDetails
                                            JOIN @SupplierZoneDetails szd on szd.SupplierZoneId = supplierZoneDetails.SupplierZoneId";

        #endregion
    }
}