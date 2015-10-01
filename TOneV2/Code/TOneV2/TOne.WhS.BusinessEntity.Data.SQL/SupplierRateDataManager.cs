using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierRateDataManager : BaseSQLDataManager, ISupplierRateDataManager
    {
        public SupplierRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
          
        }
        public List<SupplierRate> GetSupplierRates(DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetByDate", SupplierRateMapper, minimumDate);
        }
        SupplierRate SupplierRateMapper(IDataReader reader)
        {
            SupplierRate supplierRate = new SupplierRate
            {
                NormalRate = GetReaderValue<decimal>(reader, "Rate"),
                SupplierRateId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = GetReaderValue<int>(reader, "BED"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED")
            };
            return supplierRate;
        }
    }
}
