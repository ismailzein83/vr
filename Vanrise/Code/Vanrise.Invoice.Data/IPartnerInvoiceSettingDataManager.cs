using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data
{
    public interface IPartnerInvoiceSettingDataManager:IDataManager
    {
        List<PartnerInvoiceSetting> GetPartnerInvoiceSettings();
        bool ArePartnerInvoiceSettingsUpdated(ref object updateHandle);
        bool InsertPartnerInvoiceSetting(PartnerInvoiceSetting partnerInvoiceSetting);
        bool UpdatePartnerInvoiceSetting(PartnerInvoiceSetting partnerInvoiceSetting);
        bool DeletePartnerInvoiceSetting(Guid partnerInvoiceSettingId);

    }
}
