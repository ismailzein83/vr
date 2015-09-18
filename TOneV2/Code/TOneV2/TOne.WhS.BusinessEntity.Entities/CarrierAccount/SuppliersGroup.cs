using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SuppliersGroup
    {
        public abstract List<CarrierAccount> GetSuppliers();
    }

    public class SelectiveSuppliers : SuppliersGroup
    {
        public List<int> SupplierIds { get; set; }

        public override List<CarrierAccount> GetSuppliers()
        {
            ICarrierAccountManager carrierAccountManager = BEManagerFactory.GetManager<ICarrierAccountManager>();
            return carrierAccountManager.GetSuppliers(this.SupplierIds);
        }
    }

    public class AllSuppliers : SuppliersGroup
    {
        public override List<CarrierAccount> GetSuppliers()
        {
            ICarrierAccountManager carrierAccountManager = BEManagerFactory.GetManager<ICarrierAccountManager>();
            return carrierAccountManager.GetAllSuppliers();
        }
    }
}
