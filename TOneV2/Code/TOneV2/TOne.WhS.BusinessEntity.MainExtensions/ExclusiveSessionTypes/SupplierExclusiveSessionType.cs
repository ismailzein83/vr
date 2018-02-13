using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using TOne.WhS.BusinessEntity.Business;



namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class SupplierExclusiveSessionType : VRExclusiveSessionTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("80453eca-c1b1-431d-8c56-9bb92ee0551c"); }
        }

        public override bool DoesUserHaveAdminAccess(IVRExclusiveSessionDoesUserHaveAdminAccessContext context)
        {
            throw new NotImplementedException();
        }

        public override bool DoesUserHaveTakeAccess(IVRExclusiveSessionDoesUserHaveTakeAccessContext context)
        {
            Vanrise.Security.Business.SecurityManager securityManager = new Vanrise.Security.Business.SecurityManager();
            return securityManager.HasPermissionToActions("WhS_SupPL/SupplierPriceListTemplate/GetSupplierPriceListTemplateBySupplierId", context.UserId);
        }

        public override string GetTargetName(IVRExclusiveSessionGetTargetNameContext context)
        {
            var carrierAccountManager = new CarrierAccountManager();
            int carrierAccountId;
            context.TargetId.ThrowIfNull("Target ID");

            if (Int32.TryParse(context.TargetId,out carrierAccountId))
            {
                return "Supplier Pricelist: "+carrierAccountManager.GetCarrierAccountName(carrierAccountId);
            }
            else
                throw new DataIntegrityValidationException(string.Format("Target ID is in wrong Format : {0}", context.TargetId));
        }
    }
}
