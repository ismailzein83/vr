using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Routing.Entities;
using Vanrise.Caching;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class SupplierZoneDetailsDataManager : RoutingDataManager, ISupplierZoneDetailsDataManager
    {
        public DateTime? EffectiveDate { get; set; }
        public bool? IsFuture { get; set; }

        readonly string[] columns = { "SupplierId", "SupplierZoneId", "EffectiveRateValue", "SupplierServiceIds", "ExactSupplierServiceIds", "SupplierServiceWeight", "SupplierRateId", "SupplierRateEED", "CostRateTypeRuleId", "CostRateTypeId", "VersionNumber" };
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
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        public void WriteRecordToStream(SupplierZoneDetail record, object dbApplyStream)
        {
            string supplierServiceIds = record.SupplierServiceIds != null ? string.Join(",", record.SupplierServiceIds) : null;
            string exactSupplierServiceIds = record.ExactSupplierServiceIds != null ? string.Join(",", record.ExactSupplierServiceIds) : null;

            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}", record.SupplierId, record.SupplierZoneId,
                decimal.Round(record.EffectiveRateValue, 8), supplierServiceIds, exactSupplierServiceIds, record.SupplierServiceWeight, record.SupplierRateId,
                record.SupplierRateEED.HasValue ? GetDateTimeForBCP(record.SupplierRateEED) : "", record.CostRateTypeRuleId, record.CostRateTypeId, record.VersionNumber);
        }
        public void SaveSupplierZoneDetailsForDB(List<SupplierZoneDetail> supplierZoneDetails)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SupplierZoneDetail supplierZoneDetail in supplierZoneDetails)
                WriteRecordToStream(supplierZoneDetail, dbApplyStream);
            Object preparedSupplierZoneDetails = FinishDBApplyStream(dbApplyStream);
            ApplySupplierZoneDetailsForDB(preparedSupplierZoneDetails);
        }
        public void ApplySupplierZoneDetailsForDB(object preparedSupplierZoneDetails)
        {
            InsertBulkToTable(preparedSupplierZoneDetails as BaseBulkInsertInfo);
        }
        public Dictionary<long, SupplierZoneDetail> GetCachedSupplierZoneDetails()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierZoneDetailsCacheManager>();

            return cacheManager.GetOrCreateObject("SupplierZoneDetails", SupplierZoneDetailsCacheExpirationChecker.Instance, () =>
           {
               IEnumerable<SupplierZoneDetail> supplierZoneDetails = GetItemsText(query_GetSupplierZoneDetails, SupplierZoneDetailMapper, null);
               return supplierZoneDetails.ToDictionary(itm => itm.SupplierZoneId, itm => itm);
           });
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

        #region Private Methods
        SupplierZoneDetail SupplierZoneDetailMapper(IDataReader reader)
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
                CostRateTypeRuleId = GetReaderValue<int?>(reader, "CostRateTypeRuleId"),
                CostRateTypeId = GetReaderValue<int?>(reader, "CostRateTypeId"),
                VersionNumber = (int)reader["VersionNumber"]
            };
        }

        DataTable BuildZoneIdsTable(HashSet<long> zoneIds)
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
                                                  ,zd.[CostRateTypeRuleId]
                                                  ,zd.[CostRateTypeId]
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
                                                  ,zd.[CostRateTypeRuleId]
                                                  ,zd.[CostRateTypeId]
                                                  ,zd.[VersionNumber]
                                           FROM [dbo].[SupplierZoneDetail] zd with(nolock)
                                           JOIN @ZoneList z ON z.ID = zd.SupplierZoneId";
        #endregion
        private class SupplierZoneDetailsCacheManager : BaseCacheManager
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
    }
}
