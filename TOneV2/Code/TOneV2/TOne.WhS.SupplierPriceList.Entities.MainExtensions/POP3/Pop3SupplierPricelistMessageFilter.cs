using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.MainExtensions
{
    public class Pop3SupplierPricelistMessageFilter : VRPop3MessageFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("05382832-5CBB-46C0-8214-B3B81769FB80"); }
        }

        public override bool IsApplicableFunction(VRPop3MailMessageHeader receivedMailMessageHeader)
        {
            var carrierAccountManager = new CarrierAccountManager();
            var matchedSupplier = carrierAccountManager.GetMatchedSupplier(receivedMailMessageHeader.From, receivedMailMessageHeader.Subject);
            if (matchedSupplier != null && matchedSupplier.CarrierAccountSettings != null && matchedSupplier.CarrierAccountSettings.ActivationStatus != ActivationStatus.Inactive)
                return true;
            return false;
        }
    }
}
