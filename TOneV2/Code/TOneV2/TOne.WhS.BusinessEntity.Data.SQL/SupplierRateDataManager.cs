using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierRateDataManager : BaseSQLDataManager, ISupplierRateDataManager
    {

        #region ctor/Local Variables
        public SupplierRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public SupplierRate GetSupplierRateById(long rateId)
        {
            return GetItemSP("TOneWhS_BE.sp_SupplierRate_GetByID", SupplierRateMapper, rateId);
        }
        public List<SupplierRate> GetSupplierRates(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetByDate", SupplierRateMapper, supplierId, minimumDate);
        }
        public List<SupplierRate> GetEffectiveSupplierRates(DateTime fromDate, DateTime toDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetByEffective", SupplierRateMapper, fromDate, toDate);
        }
        public bool AreSupplierRatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierRate", ref updateHandle);
        }
        public List<SupplierRate> GetEffectiveSupplierRatesBySuppliers(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos)
        {
            DataTable dtActiveSuppliers = CarrierAccountDataManager.BuildRoutingSupplierInfoTable(supplierInfos);
            return GetItemsSPCmd("TOneWhS_BE.sp_SupplierRate_GetEffectiveBySuppliers", SupplierRateMapper, (cmd) =>
            {
                //, effectiveOn, isEffectiveInFuture
                var dtPrm = new SqlParameter("@ActiveSuppliersInfo", SqlDbType.Structured);
                dtPrm.Value = dtActiveSuppliers;
                cmd.Parameters.Add(dtPrm);

                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }

        public List<SupplierRate> GetSupplierRatesInBetweenPeriod(DateTime froDateTime, DateTime tillDateTime)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetBetweenPeriod", SupplierRateMapper, froDateTime, tillDateTime);
        }

        public IEnumerable<SupplierRate> GetFilteredSupplierRates(SupplierRateQuery query, DateTime effectiveOn)
        {
            string countriesIds = null;
            if (query.CountriesIds != null && query.CountriesIds.Count() > 0)
                countriesIds = string.Join<int>(",", query.CountriesIds);

            return GetItemsSP("[TOneWhS_BE].[sp_SupplierRate_GetFiltered]", SupplierRateMapper, query.SupplierId, countriesIds, query.SupplierZoneName, effectiveOn);
        }
        public IEnumerable<SupplierRate> GetFilteredSupplierPendingRates(SupplierRateQuery query, DateTime effectiveOn)
        {
            string countriesIds = null;
            if (query.CountriesIds != null && query.CountriesIds.Count() > 0)
                countriesIds = string.Join<int>(",", query.CountriesIds);

            return GetItemsSP("[TOneWhS_BE].[sp_SupplierRate_GetPending]", SupplierRateMapper, query.SupplierId, countriesIds, query.SupplierZoneName, effectiveOn);
        }
        public IEnumerable<SupplierRate> GetSupplierRatesForZone(SupplierRateForZoneQuery query, DateTime effectiveOn)
        {
            return GetItemsSP("[TOneWhS_BE].[sp_SupplierRate_GetSupplierRatesForZone]", SupplierRateMapper, query.SupplierZoneId, effectiveOn);
        }

        public IEnumerable<SupplierRate> GetZoneRateHistory(List<long> zoneIds, List<int> countryIds, int supplierId)
        {
            string zoneIdsStr = "", countryIdsStr = "";
            if (zoneIds != null && zoneIds.Any())
                zoneIdsStr = string.Join(",", zoneIds);
            if (countryIds != null && countryIds.Any())
                countryIdsStr = string.Join(",", countryIds);
            return GetItemsSP("[TOneWhS_BE].[sp_SupplierRate_GetZoneHistory]", SupplierRateMapper, supplierId,
                zoneIdsStr, countryIdsStr);
        }

        public List<SupplierRate> GetSupplierRates(HashSet<long> supplierRateIds)
        {
            string supplierRateIdsAsString = string.Join<long>(",", supplierRateIds);
            return GetItemsSP("[TOneWhS_BE].[sp_SupplierRate_GetByIds]", SupplierRateMapper, supplierRateIdsAsString);
        }

        public DateTime? GetNextOpenOrCloseTime(DateTime effectiveDate)
        {
            object nextOpenOrCloseTimeAsObj = ExecuteScalarSP("[TOneWhS_BE].[sp_SupplierRate_GetNextOpenOrCloseTime]", effectiveDate);

            DateTime? nextOpenOrCloseTime = null;
            if (nextOpenOrCloseTimeAsObj != DBNull.Value)
                nextOpenOrCloseTime = (DateTime)nextOpenOrCloseTimeAsObj;

            return nextOpenOrCloseTime;
        }

        public object GetMaximumTimeStamp()
        {
            return ExecuteScalarSP("[TOneWhS_BE].[sp_SupplierRate_GetMaxTimeStamp]");
        }
        #endregion

        #region Private Methods
        #endregion

        #region Mappers
        SupplierRate SupplierRateMapper(IDataReader reader)
        {
            SupplierRate supplierRate = new SupplierRate
            {
                SupplierRateId = GetReaderValue<long>(reader, "ID"),
                PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                ZoneId = GetReaderValue<long>(reader, "ZoneID"),
                CurrencyId = GetReaderValue<int?>(reader, "CurrencyID"),
                Rate = GetReaderValue<decimal>(reader, "Rate"),
                RateTypeId = GetReaderValue<int?>(reader, "RateTypeID"),
                RateChange = (RateChangeType)GetReaderValue<byte>(reader, "Change"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
            return supplierRate;
        }

        #endregion

        #region State Backup Methods

        public string BackupAllDataBySupplierId(long stateBackupId, string backupDatabase, int supplierId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SupplierRate] WITH (TABLOCK)
                                            SELECT  sr.[ID], sr.[PriceListID], sr.[ZoneID], sr.[CurrencyID], sr.[Rate], sr.[RateTypeID], sr.[Change], sr.[BED], sr.[EED], sr.[SourceID],  {1} AS StateBackupID  FROM [TOneWhS_BE].[SupplierRate] sr
                                            WITH (NOLOCK)  Inner Join [TOneWhS_BE].[SupplierZone] sz WITH (NOLOCK)  on sz.ID = sr.ZoneID
                                            Where sz.SupplierID = {2}", backupDatabase, stateBackupId, supplierId);
        }


        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SupplierRate] ([ID], [PriceListID], [ZoneID], [CurrencyID], [Rate], [BED], [EED], [SourceID], [Change], [RateTypeID])
                                            SELECT [ID], [PriceListID], [ZoneID], [CurrencyID], [Rate], [BED], [EED], [SourceID], [Change], [RateTypeID] FROM [{0}].[TOneWhS_BE_Bkup].[SupplierRate]
                                            WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }

        public string GetDeleteCommandsBySupplierId(int supplierId)
        {
            return String.Format(@"Delete sr FROM [TOneWhS_BE].[SupplierRate] sr Inner Join [TOneWhS_BE].[SupplierZone] sz on sz.ID = sr.ZoneID
                                            Where sz.SupplierID = {0}", supplierId);
        }


        #endregion
    }
}