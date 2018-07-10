using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public class GetSupplierSettings : CodeActivity 
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; } 
        
        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<int?> BPBusinessRuleSetId { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            int supplierId = this.SupplierId.Get(context);
            var carrierAccountManager = new CarrierAccountManager();
            var carrierAccount = carrierAccountManager.GetCarrierAccount(supplierId);

            carrierAccount.ThrowIfNull("Carrier Account", supplierId);
            carrierAccount.SupplierSettings.ThrowIfNull("CarrierAccount.SupplierSettings", supplierId);

            this.BPBusinessRuleSetId.Set(context, carrierAccount.SupplierSettings.BPBusinessRuleSetId);
        }
    }
}
