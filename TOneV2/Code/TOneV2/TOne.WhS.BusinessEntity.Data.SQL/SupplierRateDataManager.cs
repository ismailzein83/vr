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
        public List<SupplierRate> GetEffectiveSupplierRates(DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetByEffective", SupplierRateMapper, effectiveDate);
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

        public IEnumerable<SupplierRate> GetFilteredSupplierRates(SupplierRateQuery query)
        {
            string zonesids = null;
            if (query.ZoneIds != null && query.ZoneIds.Count() > 0)
                zonesids = string.Join<int>(",", query.ZoneIds);


            return GetItemsSP("[TOneWhS_BE].[sp_SupplierRate_GetFiltered]", SupplierRateMapper, query.SupplierId, zonesids, query.EffectiveOn);
        }

        #endregion

        #region Private Methods
        #endregion

        #region Mappers
        SupplierRate SupplierRateMapper(IDataReader reader)
        {
            SupplierRate supplierRate = new SupplierRate
            {
                Rate = GetReaderValue<decimal>(reader, "Rate"),
                RateTypeId = GetReaderValue<int?>(reader, "RateTypeID"),
                SupplierRateId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                CurrencyId = GetReaderValue<int?>(reader, "CurrencyId"),
                RateChange = (RateChangeType)GetReaderValue<byte>(reader, "Change")
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

        #endregion

    }
}
