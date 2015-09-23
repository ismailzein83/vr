using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SelectiveSuppliersBehavior : SupplierGroupBehavior
    {
        public override List<CarrierAccount> GetSuppliers(SupplierGroupSettings settings)
        {
            SelectiveSupplierSettings selectiveSettings = settings as SelectiveSupplierSettings;

            ICarrierAccountManager carrierAccountManager = BEManagerFactory.GetManager<ICarrierAccountManager>();
            return carrierAccountManager.GetSuppliers(selectiveSettings.SupplierIds);
        }
    }
}
