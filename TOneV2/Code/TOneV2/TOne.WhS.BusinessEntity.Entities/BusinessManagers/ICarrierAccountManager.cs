using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ICarrierAccountManager : IBEManager
    {
        List<CarrierAccount> GetAllSuppliers();

        List<CarrierAccount> GetSuppliers(IEnumerable<int> suppliersIds);
    }
}
