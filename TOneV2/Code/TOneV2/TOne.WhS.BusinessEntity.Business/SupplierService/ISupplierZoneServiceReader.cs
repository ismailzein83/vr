using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public interface ISupplierZoneServiceReader
    {
        SupplierDefaultService GetSupplierDefaultService(int supplierId);

        SupplierZoneServicesByZone GetSupplierZoneServicesByZone(int supplierId);
    }

    public class SupplierZoneServicesByZone : Dictionary<long, SupplierZoneService>
    {

    }
}
