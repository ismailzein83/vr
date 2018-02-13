using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;
using TOne.WhS.Routing.Business;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class SupplierZoneDetailsDataManager : RoutingDataManager, ISupplierZoneDetailsDataManager
    {
        public DateTime? EffectiveDate { get; set; }
        public bool? IsFuture { get; set; }
        Vanrise.Rules.RuleTree[] _ruleTreesForRouteOptions = new RouteOptionRuleManager().GetRuleTreesByPriorityForCustomerRoutes();
        Guid blockedRuleConfigId = new Guid("5a998636-0de9-4654-b430-c24805dd78d9");

        readonly string[] columns = { "SupplierId", "SupplierZoneId", "EffectiveRateValue", "SupplierServiceIds", "ExactSupplierServiceIds", "SupplierServiceWeight", "SupplierRateId", "SupplierRateEED" };
        readonly string[] zoneRatesColumns = { "ZoneID", "SupplierID", "CustomerID", "ServicesFlag", "ProfileId", "ActiveRate", "IsTOD", "IsBlock", "CodeGroup" };

        Dictionary<int, CarrierAccount> _allCarriers;
        Dictionary<int, CarrierProfile> _allCarrierProfiles;
        Dictionary<long, SupplierZone> _allSupplierZones;
        Dictionary<int, ZoneServiceConfig> _allZoneServiceConfigs;

        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();
        CarrierProfileManager _carrierProfileManager = new CarrierProfileManager();
        SupplierZoneManager _supplierZoneManager = new SupplierZoneManager();
        ZoneServiceConfigManager _zoneServiceConfigManager = new ZoneServiceConfigManager();

        public object FinishDBApplyStream(object dbApplyStream)
        {
            SupplierZoneDetailBulkInsert supplierZoneDetailBulkInsert = dbApplyStream as SupplierZoneDetailBulkInsert;

            supplierZoneDetailBulkInsert.SupplierZoneDetailStreamForBulkInsert.Close();
            supplierZoneDetailBulkInsert.ZoneRateStreamForBulkInsert.Close();

            SupplierZoneDetailBulkInsertInfo supplierZoneDetailBulkInsertInfo = new SupplierZoneDetailBulkInsertInfo();

            supplierZoneDetailBulkInsertInfo.SupplierZoneDetailStreamForBulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[SupplierZoneDetail]",
                Stream = supplierZoneDetailBulkInsert.SupplierZoneDetailStreamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };

            supplierZoneDetailBulkInsertInfo.ZoneRateStreamForBulkInsertInfo = new StreamBulkInsertInfo
            {
                TableName = "[dbo].[ZoneRates_Temp]",
                Stream = supplierZoneDetailBulkInsert.ZoneRateStreamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = zoneRatesColumns,
            };

            return supplierZoneDetailBulkInsertInfo;
        }
        public object InitialiazeStreamForDBApply()
        {
            SupplierZoneDetailBulkInsert supplierZoneDetailBulkInsert = new SupplierZoneDetailBulkInsert();
            supplierZoneDetailBulkInsert.SupplierZoneDetailStreamForBulkInsert = base.InitializeStreamForBulkInsert();
            supplierZoneDetailBulkInsert.ZoneRateStreamForBulkInsert = base.InitializeStreamForBulkInsert();
            return supplierZoneDetailBulkInsert;
        }
        public void WriteRecordToStream(SupplierZoneDetail record, object dbApplyStream)
        {
            if (_allCarriers == null)
                _allCarriers = _carrierAccountManager.GetCachedCarrierAccounts();
            if (_allSupplierZones == null)
                _allSupplierZones = _supplierZoneManager.GetCachedSupplierZones();
            if (_allCarrierProfiles == null)
                _allCarrierProfiles = _carrierProfileManager.GetCachedCarrierProfiles();
            if (_allZoneServiceConfigs == null)
                _allZoneServiceConfigs = _zoneServiceConfigManager.GetCachedZoneServiceConfigs();

            int serviceflag = GetServiceFlag(record.SupplierServiceIds, _allZoneServiceConfigs);

            string supplierServiceIds = record.SupplierServiceIds != null ? string.Join(",", record.SupplierServiceIds) : null;
            string exactSupplierServiceIds = record.ExactSupplierServiceIds != null ? string.Join(",", record.ExactSupplierServiceIds) : null;

            SupplierZoneDetailBulkInsert supplierZoneDetailBulkInsert = dbApplyStream as SupplierZoneDetailBulkInsert;
            supplierZoneDetailBulkInsert.SupplierZoneDetailStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", record.SupplierId, record.SupplierZoneId,
                decimal.Round(record.EffectiveRateValue, 8), supplierServiceIds, exactSupplierServiceIds, record.SupplierServiceWeight, record.SupplierRateId, 
                record.SupplierRateEED.HasValue ? GetDateTimeForBCP(record.SupplierRateEED) : "");

            if (!IsFuture.HasValue)
                throw new ArgumentNullException("IsFuture");

            if (!IsFuture.Value && !EffectiveDate.HasValue)
                throw new ArgumentNullException("EffectiveDate");

            var option = new RouteOptionRuleTarget
            {
                RouteTarget = new RouteRuleTarget(),
                SupplierId = record.SupplierId,
                SupplierZoneId = record.SupplierZoneId,
                EffectiveOn = EffectiveDate,
                IsEffectiveInFuture = IsFuture.Value,
            };

            RouteOptionRule matchRule = GetRouteOptionRule(option);

            if (matchRule == null || matchRule.Settings.ConfigId != blockedRuleConfigId)
            {
                CarrierAccount supplier = _allCarriers.GetRecord(record.SupplierId);
                CarrierProfile profile = _allCarrierProfiles.GetRecord(supplier.CarrierProfileId);
                SupplierZone supplierZone = _allSupplierZones.GetRecord(record.SupplierZoneId);
                supplierZoneDetailBulkInsert.ZoneRateStreamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}", supplierZone.SourceId, supplier.SourceId, "SYS", serviceflag, profile.SourceId, decimal.Round(record.EffectiveRateValue, 8), 0, 0, string.Empty);
            }
        }

        public void ApplySupplierZoneDetailsForDB(object preparedSupplierZoneDetails)
        {
            SupplierZoneDetailBulkInsertInfo supplierZoneDetailBulkInsertInfo = preparedSupplierZoneDetails as SupplierZoneDetailBulkInsertInfo;
            Parallel.For(0, 2, (i) =>
            {
                switch (i)
                {
                    case 0: InsertBulkToTable(supplierZoneDetailBulkInsertInfo.SupplierZoneDetailStreamForBulkInsertInfo); break;
                    case 1: InsertBulkToTable(supplierZoneDetailBulkInsertInfo.ZoneRateStreamForBulkInsertInfo); break;
                }
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

        public IEnumerable<SupplierZoneDetail> GetSupplierZoneDetails()
        {
            throw new NotImplementedException();
        }

        #region Private Motheds
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
                SupplierRateEED = GetReaderValue<DateTime?>(reader, "SupplierRateEED")
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
                                           FROM [dbo].[SupplierZoneDetail] zd with(nolock)
                                           JOIN @ZoneList z ON z.ID = zd.SupplierZoneId";

        #endregion

        private class SupplierZoneDetailBulkInsert
        {
            public StreamForBulkInsert SupplierZoneDetailStreamForBulkInsert { get; set; }
            public StreamForBulkInsert ZoneRateStreamForBulkInsert { get; set; }
        }

        private class SupplierZoneDetailBulkInsertInfo
        {
            public StreamBulkInsertInfo SupplierZoneDetailStreamForBulkInsertInfo { get; set; }
            public StreamBulkInsertInfo ZoneRateStreamForBulkInsertInfo { get; set; }
        }

        private RouteOptionRule GetRouteOptionRule(RouteOptionRuleTarget targetOption)
        {
            if (_ruleTreesForRouteOptions != null)
            {
                foreach (var ruleTree in _ruleTreesForRouteOptions)
                {
                    var matchRule = ruleTree.GetMatchRule(targetOption) as RouteOptionRule;
                    if (matchRule != null)
                        return matchRule;
                }
            }
            return null;
        }


        public void UpdateSupplierZoneDetails(List<SupplierZoneDetail> supplierZoneDetails)
        {
            throw new NotImplementedException();
        }


        public List<SupplierZoneDetail> GetSupplierZoneDetailsAfterVersionNumber(int versionNumber)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<SupplierZoneDetail> GetSupplierZoneDetailsByCode(string code)
        {
            throw new NotImplementedException();
        }
    }
}
