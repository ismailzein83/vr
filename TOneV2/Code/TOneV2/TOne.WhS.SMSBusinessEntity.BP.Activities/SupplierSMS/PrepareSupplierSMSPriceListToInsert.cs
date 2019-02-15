using System;
using System.Activities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SMSBusinessEntity.BP.Activities
{

    public sealed class PrepareSupplierSMSPriceListToInsert : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<SupplierSMSRateDraft> SupplierSMSRateDraft { get; set; }
        
        [RequiredArgument]
        public OutArgument<SupplierSMSPriceList> SupplierSMSPriceList { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            SupplierSMSRateDraft supplierSMSRateDraft = this.SupplierSMSRateDraft.Get(context.ActivityContext);
            long processInstanceID = context.ActivityContext.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            int userID = context.ActivityContext.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
            int supplierID = supplierSMSRateDraft.SupplierID;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            VRTimeZoneManager timeZoneManager = new VRTimeZoneManager();
            VRTimeZone supplierTimeZone = timeZoneManager.GetVRTimeZone(carrierAccountManager.GetSupplierTimeZoneId(supplierID));

            supplierTimeZone.ThrowIfNull("SupplierTimeZone", supplierID);
            supplierTimeZone.Settings.ThrowIfNull("SupplierTimeZoneSettings", supplierID);

                if (supplierSMSRateDraft.EffectiveDate == DateTime.MinValue)
                    throw new VRBusinessException("Invalid Effective Date for this supplier");

            DateTime effectiveOn = supplierSMSRateDraft.EffectiveDate.Subtract(supplierTimeZone.Settings.Offset);

            SupplierSMSPriceList SupplierSMSPriceList = new SupplierSMSPriceListManager().CreateSupplierSMSPriceList(supplierSMSRateDraft.SupplierID, supplierSMSRateDraft.CurrencyId, effectiveOn, processInstanceID, userID);

            this.SupplierSMSPriceList.Set(context.ActivityContext, SupplierSMSPriceList);
        }
    }
}
