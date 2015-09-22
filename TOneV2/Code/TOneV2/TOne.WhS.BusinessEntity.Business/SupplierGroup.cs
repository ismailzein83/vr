using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public abstract class SupplierGroup
    {
        public abstract List<CarrierAccount> GetSuppliers(SupplierGroupSettings settings);
    }

    public class SelectiveSuppliers : SupplierGroup
    {
        public override List<CarrierAccount> GetSuppliers(SupplierGroupSettings settings)
        {
            SelectiveSupplierSettings selectiveSettings = settings as SelectiveSupplierSettings;

            ICarrierAccountManager carrierAccountManager = BEManagerFactory.GetManager<ICarrierAccountManager>();
            return carrierAccountManager.GetSuppliers(selectiveSettings.SupplierIds);
        }
    }

    public class AllSuppliers : SupplierGroup
    {
        public override List<CarrierAccount> GetSuppliers(SupplierGroupSettings settings)
        {
            ICarrierAccountManager carrierAccountManager = BEManagerFactory.GetManager<ICarrierAccountManager>();
            return carrierAccountManager.GetAllSuppliers();
        }
    }
}
