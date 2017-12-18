using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using System.ComponentModel;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class SaleAreaExclusiveSessionType : VRExclusiveSessionTypeExtendedSettings
    {

        public override Guid ConfigId
        {
            get { return new Guid("7a2c8c43-7242-4fb0-9f05-43d8395d0276"); }
        }

        public override bool DoesUserHaveAdminAccess(IVRExclusiveSessionDoesUserHaveAdminAccessContext context)
        {
            throw new NotImplementedException();
        }

        public override bool DoesUserHaveTakeAccess(IVRExclusiveSessionDoesUserHaveTakeAccessContext context)
        {
            Vanrise.Security.Business.SecurityManager securityManager = new Vanrise.Security.Business.SecurityManager();
            return securityManager.HasPermissionToActions("WhS_Sales/RatePlan/GetRatePlanSettingsData", context.UserId);
        }

        public override string GetTargetName(IVRExclusiveSessionGetTargetNameContext context)
        {
            var carrierAccountManager = new CarrierAccountManager();
            var sellingProductManager = new SellingProductManager();
            var sellingNumberPlanManager = new SellingNumberPlanManager();

            context.TargetId.ThrowIfNull("Target ID");

            string[] targetIdComponents = context.TargetId.Split('_');
            if (targetIdComponents.Length !=2)
                return null;

            string targetPrefix = targetIdComponents[0];
            string id = targetIdComponents[1];

            int targetIdValue;

            if (!Int32.TryParse(id, out targetIdValue))
                throw new DataIntegrityValidationException(string.Format("Traget ID is in wrong Format : {0}", context.TargetId));

            ExclusiveSessionTargetIdPrefixEnum prefixConstant;
            if (!Enum.TryParse(targetPrefix, out prefixConstant))
                throw new DataIntegrityValidationException(string.Format("Traget ID is in wrong Format : {0}", context.TargetId));

            switch (prefixConstant)
            {
                case ExclusiveSessionTargetIdPrefixEnum.Customer:
                    return "Customer: "+carrierAccountManager.GetCarrierAccountName(targetIdValue);
                case ExclusiveSessionTargetIdPrefixEnum.SellingProduct:
                    return "SellingProduct: " + sellingProductManager.GetSellingProductName(targetIdValue);
                case ExclusiveSessionTargetIdPrefixEnum.NumberingPlan:
                    return "NumberingPlan: " + sellingNumberPlanManager.GetSellingNumberPlanName(targetIdValue);
            }
             return null;
        }

        public enum ExclusiveSessionTargetIdPrefixEnum
        {
            [Description("Customer")]
            Customer = 0,
            [Description("SellingProduct")]
            SellingProduct = 1,
            [Description("NumberingPlan")]
            NumberingPlan = 2,
        }
    }
}