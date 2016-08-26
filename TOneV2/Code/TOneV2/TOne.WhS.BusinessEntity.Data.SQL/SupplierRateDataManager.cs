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
        public List<SupplierRate> GetSupplierRates(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetByDate", SupplierRateMapper, supplierId, minimumDate);
        }
        public List<SupplierRate> GetEffectiveSupplierRates(int supplierId, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetBySupplierAndEffective", SupplierRateMapper, supplierId, effectiveDate);
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
        public List<SupplierRate> GetAllSupplierRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetAll", SupplierRateMapper, effectiveOn, isEffectiveInFuture);
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
                NormalRate = GetReaderValue<decimal>(reader, "NormalRate"),
                OtherRates =
                    reader["OtherRates"] as string != null
                        ? Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal>>(reader["OtherRates"] as string)
                        : null,
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

    }
}
