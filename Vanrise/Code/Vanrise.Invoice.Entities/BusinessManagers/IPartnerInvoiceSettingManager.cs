using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public interface IPartnerInvoiceSettingManager:IBusinessManager
    {
        PartnerInvoiceSetting GetPartnerInvoiceSettingByPartnerId(string partnerId);
        T GetPartnerInvoiceSettingDetailByType<T>(Guid partnerInvoiceSettingId) where T : InvoiceSettingPart;
    }
}
