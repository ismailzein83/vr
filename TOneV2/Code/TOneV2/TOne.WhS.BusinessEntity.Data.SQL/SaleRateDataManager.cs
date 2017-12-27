using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleRateDataManager : BaseSQLDataManager, ISaleRateDataManager
    {
        #region Fields / Constructors
        public SaleRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }

        #endregion

        #region Public Methods

        public SaleRate GetSaleRateById(long rateId)
        {
            return GetItemSP("TOneWhS_BE.sp_SaleRate_GetByID", SaleRateMapper, rateId);
        }

        public List<SaleRate> GetEffectiveSaleRates(DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetByEffective", SaleRateMapper, effectiveOn);
        }

        public List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetEffectiveAfter", SaleRateMapper, sellingNumberPlanId, minimumDate);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetEffectiveAfterByOwner", SaleRateMapper, ownerType, ownerId, minimumDate);
        }

        public List<SaleRate> GetSaleRatesInBetweenPeriod(DateTime fromTime, DateTime tillTime)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetBetweenPeriod", SaleRateMapper, fromTime, tillTime);
        }

        public List<SaleRate> GetEffectiveSaleRateByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DataTable saleRateOwners = BuildRoutingOwnerInfoTable(customerInfos);
            return GetItemsSPCmd("[TOneWhS_BE].[sp_SaleRate_GetEffectiveByOwner]", SaleRateMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@SaleRateOwner", SqlDbType.Structured);
                dtPrm.Value = saleRateOwners;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }

        public List<SaleRate> GetEffectiveAfterByMultipleOwners(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime effectiveAfter)
        {
            DataTable saleRateOwners = BuildRoutingOwnerInfoTable(customerInfos);
            return GetItemsSPCmd("[TOneWhS_BE].[sp_SaleRate_GetEffectiveAfterByMultipleOwners]", SaleRateMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@SaleRateOwner", SqlDbType.Structured);
                dtPrm.Value = saleRateOwners;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@EffectiveAfter", effectiveAfter));
            });
        }
        public bool AreSaleRatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleRate", ref updateHandle);
        }

        public IEnumerable<SaleRate> GetExistingRatesByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED)
        {
            string zoneIdsParameter = string.Join(",", zoneIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetExistingByZoneIDs", SaleRateMapper, ownerType, ownerId, zoneIdsParameter, minEED);
        }

        internal static DataTable BuildRoutingOwnerInfoTable(IEnumerable<RoutingCustomerInfoDetails> customerInfos)
        {
            HashSet<int> addedSellingProductIds = new HashSet<int>();
            DataTable dtRoutingInfos = GetRoutingOwnerInfoTable();
            dtRoutingInfos.BeginLoadData();
            foreach (var c in customerInfos)
            {
                DataRow drCustomer = dtRoutingInfos.NewRow();
                drCustomer["OwnerId"] = c.CustomerId;
                drCustomer["OwnerTpe"] = (int)SalePriceListOwnerType.Customer;
                dtRoutingInfos.Rows.Add(drCustomer);

                if (addedSellingProductIds.Contains(c.SellingProductId))
                    continue;

                DataRow drSP = dtRoutingInfos.NewRow();
                drSP["OwnerId"] = c.SellingProductId;
                drSP["OwnerTpe"] = (int)SalePriceListOwnerType.SellingProduct;
                dtRoutingInfos.Rows.Add(drSP);
                addedSellingProductIds.Add(c.SellingProductId);
            }
            dtRoutingInfos.EndLoadData();
            return dtRoutingInfos;
        }

        private static DataTable GetRoutingOwnerInfoTable()
        {
            DataTable dtRoutingInfos = new DataTable();
            dtRoutingInfos.Columns.Add("OwnerId", typeof(Int32));
            dtRoutingInfos.Columns.Add("OwnerTpe", typeof(Int32));
            return dtRoutingInfos;
        }

        public IEnumerable<SaleRate> GetFutureSaleRates(SalePriceListOwnerType ownerType, int ownerId)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetFutureByOwner", SaleRateMapper, ownerType, ownerId);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfterByOwnerAndZones(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime effectiveOn)
        {
            if (zoneIds == null)
                throw new Vanrise.Entities.MissingArgumentValidationException("zoneIds");
            string zoneIdsAsString = string.Join(",", zoneIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetEffectiveAfterByOwnerAndZones", SaleRateMapper, ownerType, ownerId, zoneIdsAsString, effectiveOn);
        }

        public IEnumerable<SaleRate> GetZoneRatesBySellingProduct(int sellingProductId, IEnumerable<long> saleZoneIds)
        {
            string saleZoneIdsAsString = (saleZoneIds != null) ? string.Join(",", saleZoneIds) : null;
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetBySellingProduct", SaleRateMapper, sellingProductId, saleZoneIdsAsString);
        }

        public IEnumerable<SaleRate> GetZoneRatesBySellingProducts(IEnumerable<int> sellingProductIds, IEnumerable<long> saleZoneIds)
        {
            string sellingProductIdsAsString = (sellingProductIds != null) ? string.Join(",", sellingProductIds) : null;
            string saleZoneIdsAsString = (saleZoneIds != null) ? string.Join(",", saleZoneIds) : null;
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetBySellingProducts", SaleRateMapper, sellingProductIdsAsString, saleZoneIdsAsString);
        }

        public IEnumerable<SaleRate> GetAllSaleRatesByOwner(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> saleZoneIds, bool getNormalRates, bool getOtherRates)
        {
            string saleZoneIdsAsString = (saleZoneIds != null) ? string.Join(",", saleZoneIds) : null;
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetAllByOwner", SaleRateMapper, ownerType, ownerId, saleZoneIdsAsString, getNormalRates, getOtherRates);
        }

        public IEnumerable<SaleRate> GetAllSaleRatesBySellingProductAndCustomer(IEnumerable<long> saleZoneIds, int sellingProductId, int customerId, bool getNormalRates, bool getOtherRates)
        {
            string saleZoneIdsAsString = (saleZoneIds != null) ? string.Join(",", saleZoneIds) : null;
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetAllBySellingProductAndCustomer", SaleRateMapper, saleZoneIdsAsString, sellingProductId, customerId, getNormalRates, getOtherRates);
        }

        public IEnumerable<SaleRate> GetSaleRatesEffectiveAfterByOwnersAndZones(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds, IEnumerable<long> zoneIds, DateTime minimumDate)
        {
            string ownerIdsAsString = (ownerIds != null) ? string.Join(",", ownerIds) : null;
            string zoneIdsAsString = (zoneIds != null) ? string.Join(",", zoneIds) : null;
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetEffectiveAfterByOwnersAndZones", SaleRateMapper, ownerType, ownerIdsAsString, zoneIdsAsString, minimumDate);
        }

        public DateTime? GetNextOpenOrCloseTime(DateTime effectiveDate)
        {
            object nextOpenOrCloseTimeAsObj = ExecuteScalarSP("[TOneWhS_BE].[sp_SaleRate_GetNextOpenOrCloseTime]", effectiveDate);

            DateTime? nextOpenOrCloseTime = null;
            if (nextOpenOrCloseTimeAsObj != DBNull.Value)
                nextOpenOrCloseTime = (DateTime)nextOpenOrCloseTimeAsObj;

            return nextOpenOrCloseTime;
        }

        public object GetMaximumTimeStamp()
        {
            return ExecuteScalarSP("[TOneWhS_BE].[sp_SaleRate_GetMaxTimeStamp]");
        }

        public IEnumerable<SaleRate> GetAllSaleRatesByOwners(IEnumerable<int> sellingProductIds, IEnumerable<int> customerIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetAllByOwners", SaleRateMapper, string.Join(",", sellingProductIds), string.Join(",", customerIds), string.Join(",", zoneIds), getNormalRates, getOtherRates);
        }

        public IEnumerable<SaleRate> GetAllSaleRatesByOwnerType(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetAllByOwnerType", SaleRateMapper, ownerType, string.Join(",", ownerIds), string.Join(",", zoneIds), getNormalRates, getOtherRates);
        }

        #endregion

        #region Mappers

        private SaleRate SaleRateMapper(IDataReader reader)
        {
            SaleRate saleRate = new SaleRate();

            saleRate.SaleRateId = (long)reader["ID"];
            saleRate.ZoneId = (long)reader["ZoneID"];
            saleRate.PriceListId = (int)reader["PriceListID"];

            saleRate.RateTypeId = GetReaderValue<int?>(reader, "RateTypeID");
            saleRate.Rate = (decimal)reader["Rate"];

            saleRate.BED = (DateTime)reader["BED"];
            saleRate.EED = GetReaderValue<DateTime?>(reader, "EED");

            saleRate.RateChange = (Entities.RateChangeType)GetReaderValue<byte>(reader, "Change");
            saleRate.CurrencyId = GetReaderValue<int?>(reader, "CurrencyId");
            return saleRate;
        }

        #endregion

        #region State Backup Methods

        public string BackupAllDataBySellingNumberingPlanId(long stateBackupId, string backupDatabase, int sellingNumberPlanId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SaleRate] WITH (TABLOCK)
                                            SELECT sr.[ID], sr.[PriceListID], sr.[ZoneID], sr.[CurrencyID], sr.[RateTypeID], sr.[Rate], sr.[BED], sr.[EED], sr.[SourceID], sr.[Change], {1} AS StateBackupID  FROM [TOneWhS_BE].[SaleRate]
                                            sr WITH (NOLOCK) Inner Join [TOneWhS_BE].SaleZone sz WITH (NOLOCK) on sr.ZoneID = sz.ID
                                            Where sz.SellingNumberPlanID = {2}", backupDatabase, stateBackupId, sellingNumberPlanId);
        }

        public string BackupAllDataByOwner(long stateBackupId, string backupDatabase, IEnumerable<int> ownerIds, int ownerType)
        {
            string ownerIdsString = null;
            if (ownerIds != null && ownerIds.Any())
                ownerIdsString = string.Join<int>(",", ownerIds);

            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SaleRate] WITH (TABLOCK)
                                           SELECT sr.[ID], sr.[PriceListID], sr.[ZoneID], sr.[CurrencyID], sr.[RateTypeID], sr.[Rate], sr.[BED], sr.[EED], sr.[SourceID], sr.[Change], {1} AS StateBackupID FROM [TOneWhS_BE].[SaleRate]
                                           sr WITH (NOLOCK) Inner Join [TOneWhS_BE].[SalePriceList] pl WITH (NOLOCK) on sr.PriceListID = pl.ID
                                           Where pl.OwnerId IN ({2}) and pl.OwnerType = {3}", backupDatabase, stateBackupId, ownerIdsString, ownerType);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SaleRate] ([ID], [PriceListID], [ZoneID], [CurrencyID], [RateTypeID], [Rate], [BED], [EED], [SourceID], [Change])
                                            SELECT [ID], [PriceListID], [ZoneID], [CurrencyID], [RateTypeID], [Rate], [BED], [EED], [SourceID], [Change] FROM [{0}].[TOneWhS_BE_Bkup].[SaleRate]
                                            WITH (NOLOCK) Where StateBackupID = {1}  AND zoneid in ( select id from TOneWhS_BE.SaleZone)", backupDatabase, stateBackupId);
        }

        public string GetDeleteCommandsBySellingNumberPlanId(long sellingNumberPlanId)
        {
            return String.Format(@"DELETE sr FROM [TOneWhS_BE].[SaleRate] sr Inner Join [TOneWhS_BE].SaleZone sz  on sr.ZoneID = sz.ID
                                            Where sz.SellingNumberPlanID = {0}", sellingNumberPlanId);
        }

        public string GetDeleteCommandsByOwner(IEnumerable<int> ownerIds, int ownerType)
        {
            string deleteQuery = string.Empty;
            if (ownerIds != null && ownerIds.Any())
            {
                string ownerIdsString = string.Join(",", ownerIds);
                deleteQuery = String.Format(@"DELETE sr FROM [TOneWhS_BE].[SaleRate] sr Inner Join [TOneWhS_BE].[SalePriceList] pl  on sr.PriceListID = pl.ID
                                           Where pl.OwnerId IN ( {0} ) and pl.OwnerType = {1}", ownerIdsString, ownerType);
            }
            return deleteQuery;
        }

        #endregion
    }
}
