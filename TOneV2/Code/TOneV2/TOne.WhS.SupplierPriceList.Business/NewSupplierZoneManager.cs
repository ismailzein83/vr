using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class NewSupplierZoneManager
    {
        public void Insert(int supplierId, int priceListId, IEnumerable<NewZone> zonesList)
        {
            INewSupplierZoneDataManager dataManager = SupPLDataManagerFactory.GetDataManager<INewSupplierZoneDataManager>();
            dataManager.Insert(supplierId, priceListId, zonesList);
        }
    }
}
