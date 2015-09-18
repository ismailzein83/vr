using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleZoneDataManager : BaseTOneDataManager, ISaleZoneDataManager
    {
        public SaleZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
          
        }
        public List<SaleZone> GetSaleZones(int packageId)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZone_GetAll", SaleZoneMapper, packageId);
        }
        SaleZone SaleZoneMapper(IDataReader reader)
        {
            SaleZone saleZonePackage = new SaleZone
            {
                SaleZoneId = (int)reader["ID"],
                SaleZonePackageId = (int)reader["ID"],
                Name = reader["Name"] as string,
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime>(reader, "EED")
            };
            return saleZonePackage;
        }
    }
}
