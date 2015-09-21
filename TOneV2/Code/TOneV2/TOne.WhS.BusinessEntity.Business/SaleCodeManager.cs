using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleCodeManager
    {
        public List<SaleCode> GetSaleCodesByZoneID(long zoneID)
        {
            ISaleCodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleCodeDataManager>();
            return dataManager.GetSaleCodesByZoneID(zoneID);
        }
    }
}
