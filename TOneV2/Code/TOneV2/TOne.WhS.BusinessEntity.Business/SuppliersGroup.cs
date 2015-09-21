using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public abstract class SuppliersGroup
    {
        public abstract List<CarrierAccount> GetSuppliers(SuppliersGroupSettings settings);
    }

    public class SelectiveSuppliers : SuppliersGroup
    {
        public override List<CarrierAccount> GetSuppliers(SuppliersGroupSettings settings)
        {
            SelectiveSuppliersSettings selectiveSettings = settings as SelectiveSuppliersSettings;

            ICarrierAccountManager carrierAccountManager = BEManagerFactory.GetManager<ICarrierAccountManager>();
            return carrierAccountManager.GetSuppliers(selectiveSettings.SupplierIds);
        }
    }

    public class AllSuppliers : SuppliersGroup
    {
        public override List<CarrierAccount> GetSuppliers(SuppliersGroupSettings settings)
        {
            ICarrierAccountManager carrierAccountManager = BEManagerFactory.GetManager<ICarrierAccountManager>();
            return carrierAccountManager.GetAllSuppliers();
        }
    }
}
