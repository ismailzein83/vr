using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class PartnerManager
    {
        public InvoicePartnerManager GetPartnerDetails(Guid invoiceTypeId)
        {
            InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            if (invoiceType.Settings == null || invoiceType.Settings.ExtendedSettings ==null)
                throw new NullReferenceException("InvoicePartnerDetails");
            return invoiceType.Settings.ExtendedSettings.GetPartnerManager();
        }
        public dynamic GetPartnerInfo(Guid invoiceTypeId, string partnerId,string infoType)
        {
            var partnerSettings = GetPartnerDetails(invoiceTypeId);

            PartnerManagerInfoContext context = new PartnerManagerInfoContext
            {
                InfoType = infoType,
                PartnerId = partnerId,
                InvoicePartnerManager = partnerSettings
            };
            if (partnerSettings == null || partnerSettings.GetPartnerInfo(context) == null)
                throw new NullReferenceException("PartnerManager");
            return partnerSettings.GetPartnerInfo(context);
        }
        public dynamic GetActualPartnerId(Guid invoiceTypeId, string partnerId)
        {

            var partnerSettings = GetPartnerDetails(invoiceTypeId);

            ActualPartnerContext context = new ActualPartnerContext
            {
                PartnerId = partnerId,
            };
            if (partnerSettings == null)
                throw new NullReferenceException("PartnerManagerFQTN");
            return partnerSettings.GetActualPartnerId(context);
        }
        public int GetPartnerDuePeriod(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerDetails(invoiceTypeId);
            var partnerSettings = GetInvoicePartnerSetting(invoiceTypeId, partnerId);
            InvoicePartnerSettingPartContext invoicePartnerSettingPartContext = new InvoicePartnerSettingPartContext
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = partnerId,
                InvoiceSettingId = partnerSettings.InvoiceSetting.InvoiceSettingId
            };
            var duePeriod = invoicePartnerManager.GetInvoicePartnerSettingPart<DuePeriodInvoiceSettingPart>(invoicePartnerSettingPartContext);
            if (duePeriod != null)
                return duePeriod.DuePeriod;
            return default(int);
        }
        public string GetPartnerSerialNumberPattern(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerDetails(invoiceTypeId);
            var partnerSettings = GetInvoicePartnerSetting(invoiceTypeId, partnerId);
            InvoicePartnerSettingPartContext invoicePartnerSettingPartContext = new InvoicePartnerSettingPartContext
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = partnerId,
                InvoiceSettingId = partnerSettings.InvoiceSetting.InvoiceSettingId
            };
            var serialNumberPartern = invoicePartnerManager.GetInvoicePartnerSettingPart<SerialNumberPatternInvoiceSettingPart>(invoicePartnerSettingPartContext);
            if (serialNumberPartern != null)
                return serialNumberPartern.SerialNumberPattern;
            return null;
        }
        public InvoicePartnerSettings GetInvoicePartnerSetting(Guid invoiceTypeId, string partnerId)
        {
            var partnerSettings = GetPartnerDetails(invoiceTypeId);
            Context.InvoicePartnerSettingsContext context = new Context.InvoicePartnerSettingsContext
            {
                PartnerId = partnerId,
                InvoiceTypeId = invoiceTypeId
            };
            if (partnerSettings == null)
                throw new NullReferenceException("partnerSettings");
            return partnerSettings.GetInvoicePartnerSettings(context);
        }
        public bool CheckInvoiceFollowBillingPeriod(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerDetails(invoiceTypeId);
            var partnerSettings = GetInvoicePartnerSetting(invoiceTypeId, partnerId);
            InvoicePartnerSettingPartContext invoicePartnerSettingPartContext = new InvoicePartnerSettingPartContext
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = partnerId,
                InvoiceSettingId = partnerSettings.InvoiceSetting.InvoiceSettingId
            };
            var billingPeriod = invoicePartnerManager.GetInvoicePartnerSettingPart<BillingPeriodInvoiceSettingPart>(invoicePartnerSettingPartContext);
            if (billingPeriod != null)
                return billingPeriod.FollowBillingPeriod;
            return false;
        }
        public BillingPeriod GetPartnerBillingPeriod(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerDetails(invoiceTypeId);
            var partnerSettings = GetInvoicePartnerSetting(invoiceTypeId, partnerId);
            InvoicePartnerSettingPartContext invoicePartnerSettingPartContext = new InvoicePartnerSettingPartContext
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = partnerId,
                InvoiceSettingId = partnerSettings.InvoiceSetting.InvoiceSettingId
            };
            var billingPeriod = invoicePartnerManager.GetInvoicePartnerSettingPart<BillingPeriodInvoiceSettingPart>(invoicePartnerSettingPartContext);
            if (billingPeriod != null)
                return billingPeriod.BillingPeriod;
            return null;
        }
    }
}
