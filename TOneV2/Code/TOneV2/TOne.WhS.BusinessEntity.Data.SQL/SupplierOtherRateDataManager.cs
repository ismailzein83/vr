using System;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierOtherRateDataManager : BaseSQLDataManager, ISupplierOtherRateDataManager
    {

        #region ctor/Local Variables
        public SupplierOtherRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<SupplierOtherRate> GetFilteredSupplierOtherRates(SupplierOtherRateQuery query)
        {
            return GetItemsSP("[TOneWhS_BE].[sp_SupplierOtherRate_GetFiltered]", SupplierOtherRateMapper, query.ZoneId, query.EffectiveOn);
        }
        public IEnumerable<SupplierOtherRate> GetSupplierOtherRates(IEnumerable<long> zoneIds, DateTime effectiveOn)
        {
            string zoneIdsString = null;
            if (zoneIds != null && zoneIds.Any())
                zoneIdsString = string.Join(",", zoneIds);

            return GetItemsSP("[TOneWhS_BE].[sp_SupplierOtherRate_GetByZoneIds]", SupplierOtherRateMapper, zoneIdsString, effectiveOn);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Mappers
        SupplierOtherRate SupplierOtherRateMapper(IDataReader reader)
        {
            SupplierOtherRate supplierOtherRate = new SupplierOtherRate
            {
                Rate = GetReaderValue<decimal>(reader, "Rate"),
                RateTypeId = GetReaderValue<int?>(reader, "RateTypeID"),
                SupplierRateId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                CurrencyId = GetReaderValue<int?>(reader, "CurrencyId"),
                RateChange = (RateChangeType)GetReaderValue<int>(reader, "Change")
            };
            return supplierOtherRate;
        }

        #endregion


    }
}
