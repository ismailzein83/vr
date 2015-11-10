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
        public List<SupplierRate> GetSupplierRates(int supplierId,DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetByDate", SupplierRateMapper,supplierId, minimumDate);
        }
        SupplierRate SupplierRateMapper(IDataReader reader)
        {
            SupplierRate supplierRate = new SupplierRate
            {
                NormalRate = GetReaderValue<decimal>(reader, "NormalRate"),
                OtherRates = reader["OtherRates"] as string!=null?Vanrise.Common.Serializer.Deserialize<Dictionary<int,decimal>>(reader["OtherRates"] as string):null,
                SupplierRateId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED")
            };
            return supplierRate;
        }


        public List<SupplierRate> GetEffectiveSupplierRates(int supplierId, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetBySupplierAndEffective", SupplierRateMapper, supplierId, effectiveDate);
        }

        public bool AreSupplierRatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierRate", ref updateHandle);
        }


        public List<SupplierRate> GetAllSupplierRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetAll", SupplierRateMapper, effectiveOn, isEffectiveInFuture);
        }
    }
}
