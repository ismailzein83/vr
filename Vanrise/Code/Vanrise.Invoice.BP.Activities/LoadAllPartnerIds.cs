using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.BusinessProcess;

namespace Vanrise.Invoice.BP.Activities
{

    public sealed class LoadAllPartnerIds : BaseCodeActivity
    {
      
        #region Arguments
        [RequiredArgument]
        public InArgument<Guid> InvoiceTypeId { get; set; }
        [RequiredArgument]
        public OutArgument<IEnumerable<string>> PartnerIds { get; set; }
        #endregion
        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            var invoiceTypeId = context.ActivityContext.GetValue(this.InvoiceTypeId);
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceTypeId);
            var partnerIds = invoiceType.Settings.ExtendedSettings.GetPartnerIds(new ExtendedSettingsPartnerIdsContext());
            this.PartnerIds.Set(context.ActivityContext, partnerIds);
        }
    }
}
