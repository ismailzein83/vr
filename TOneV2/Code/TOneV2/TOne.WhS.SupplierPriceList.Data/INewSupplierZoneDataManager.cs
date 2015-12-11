using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface INewSupplierZoneDataManager : IDataManager
    {
        void Insert(int supplierId, int priceListId, IEnumerable<NewZone> zonesList);
    }
}
