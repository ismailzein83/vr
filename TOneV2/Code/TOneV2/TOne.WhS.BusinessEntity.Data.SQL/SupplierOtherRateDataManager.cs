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
                RateChange = (RateChangeType)GetReaderValue<byte>(reader, "Change")
            };
            return supplierOtherRate;
        }

        #endregion


    }
}
