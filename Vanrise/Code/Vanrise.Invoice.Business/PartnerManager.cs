using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class PartnerManager
    {
        public dynamic GetPartnerInfo(Guid invoiceTypeId, string partnerId,string infoType)
        {
            InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            IPartnerManagerInfoContext context = new IPartnerManagerInfoContext
            {
                InfoType = infoType,
                PartnerId = partnerId,
                PartnerSettings = invoiceType.Settings.UISettings.PartnerSettings
            };
            if (invoiceType.Settings.UISettings.PartnerSettings.PartnerManagerFQTN == null)
                throw new NullReferenceException("PartnerManagerFQTN");
            return invoiceType.Settings.UISettings.PartnerSettings.PartnerManagerFQTN.GetPartnerInfo(context);
        }
    }
}
