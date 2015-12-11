using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ChangedSupplierZoneManager
    {
        public void Insert(int priceListId, IEnumerable<Entities.SPL.ChangedZone> changedZones)
        {
            IChangedSupplierZoneDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IChangedSupplierZoneDataManager>();
            dataManager.Insert(priceListId, changedZones);
        }
    }
}
