using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface IImportPriceListDataManager:IDataManager
    {
        void InsertPriceListObject(List<Zone> supplierZones, List<Code> codesToBeDeleted);
    }
}
