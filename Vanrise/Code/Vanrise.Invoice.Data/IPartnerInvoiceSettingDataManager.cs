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
        bool InsertPartnerInvoiceSetting(Guid invoicePartnerSettingId, Guid invoiceSettingId, string partnerId, PartnerInvoiceSettingDetails partnerInvoiceSettingDetails);
        bool UpdatePartnerInvoiceSetting(Guid partnerInvoiceSettingId, PartnerInvoiceSettingDetails partnerInvoiceSettingDetails);
        bool DeletePartnerInvoiceSetting(Guid partnerInvoiceSettingId);
        bool InsertOrUpdateInvoiceSetting(Guid partnerInvoiceSettingId, string partnerId, Guid invoiceSettingId);

    }
}
