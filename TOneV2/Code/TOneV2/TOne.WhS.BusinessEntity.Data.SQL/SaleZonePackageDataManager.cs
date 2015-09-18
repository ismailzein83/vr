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
    public class SaleZonePackageDataManager : BaseTOneDataManager, ISaleZonePackageDataManager
    {
        public SaleZonePackageDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }


        public List<SaleZonePackage> GetSaleZonePackages()
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZonePackage_GetAll", SaleZonePackageMapper);
           
        }

        SaleZonePackage SaleZonePackageMapper(IDataReader reader)
        {
            SaleZonePackage saleZonePackage = new SaleZonePackage
            {
                SaleZonePackageId = (int)reader["ID"],
                Name = reader["Name"] as string,
            };
            return saleZonePackage;
        }
    }
}
