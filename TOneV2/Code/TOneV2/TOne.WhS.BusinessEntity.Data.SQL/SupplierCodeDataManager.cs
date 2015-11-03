using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierCodeDataManager : BaseTOneDataManager, ISupplierCodeDataManager
    {
        public SupplierCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
          
        }

        public List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetByDate", SupplierCodeMapper, supplierId,minimumDate);
        }
        SupplierCode SupplierCodeMapper(IDataReader reader)
        {
            SupplierCode supplierCode = new SupplierCode
            {
                Code = GetReaderValue<string>(reader, "Code"),
                SupplierCodeId = GetReaderValue<long>(reader, "ID"),
                ZoneId = (long)reader["ZoneID"],
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED"),
                
            };
            return supplierCode;
        }


        public List<SupplierCode> GetSupplierCodes(int supplierId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetBySupplier", SupplierCodeMapper, supplierId, effectiveOn);
        }
    }
}
