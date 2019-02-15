using System.Activities;
using TOne.WhS.SMSBusinessEntity.BP.Arguments;
using TOne.WhS.SMSBusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.SMSBusinessEntity.BP.Activities
{
    public sealed class GetSupplierSMSRateDraftData : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<SMSSupplierRateInput> SMSSupplierRateInput { get; set; }

        [RequiredArgument]
        public OutArgument<SupplierSMSRateDraft> SupplierSMSRateDraft { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            long processDraftID = this.SMSSupplierRateInput.Get(context.ActivityContext).ProcessDraftID;

            SupplierSMSRateDraft supplierSMSRateDraft = new SupplierSMSRateDraftManager().GetSupplierSMSRateDraft(processDraftID);
            supplierSMSRateDraft.ThrowIfNull("supplierSMSRateDraft", processDraftID);

            this.SupplierSMSRateDraft.Set(context.ActivityContext, supplierSMSRateDraft);
        }
    }
}