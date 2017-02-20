using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class PartnerManager
    {
        public InvoicePartnerManager GetPartnerManager(Guid invoiceTypeId)
        {
            InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            if (invoiceType.Settings == null )
               throw new NullReferenceException("invoiceType.Settings");
             if (invoiceType.Settings.ExtendedSettings == null )
               throw new NullReferenceException("invoiceType.Settings.ExtendedSettings");
            var invoicePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
            if(invoicePartnerManager == null)
                  throw new NullReferenceException("invoicePartnerManager");
            return invoicePartnerManager;
        }
        public dynamic GetPartnerInfo(Guid invoiceTypeId, string partnerId,string infoType)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);

            PartnerManagerInfoContext context = new PartnerManagerInfoContext
            {
                InfoType = infoType,
                PartnerId = partnerId,
                InvoicePartnerManager = invoicePartnerManager
            };
            var partnerInfo = invoicePartnerManager.GetPartnerInfo(context);
            if (partnerInfo == null)
                throw new NullReferenceException("partnerInfo");
            return partnerInfo;
        }
        public dynamic GetActualPartnerId(Guid invoiceTypeId, string partnerId)
        {

            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);

            ActualPartnerContext context = new ActualPartnerContext
            {
                PartnerId = partnerId,
            };
            return invoicePartnerManager.GetActualPartnerId(context);
        }
        public int GetPartnerDuePeriod(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
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
        public int? GetPartnerTimeZone(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
            PartnerTimeZoneContext partnerTimeZoneContext = new PartnerTimeZoneContext { PartnerId = partnerId };
            return invoicePartnerManager.GetPartnerTimeZoneId(partnerTimeZoneContext);
        }
        public string GetPartnerSerialNumberPattern(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
            var partnerSettings = GetInvoicePartnerSetting(invoiceTypeId, partnerId);
            InvoicePartnerSettingPartContext invoicePartnerSettingPartContext = new InvoicePartnerSettingPartContext
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = partnerId,
                InvoiceSettingId = partnerSettings.InvoiceSetting.InvoiceSettingId
            };
            var serialNumberPartern = invoicePartnerManager.GetInvoicePartnerSettingPart<SerialNumberPatternInvoiceSettingPart>(invoicePartnerSettingPartContext) as SerialNumberPatternInvoiceSettingPart;
            if (serialNumberPartern != null)
                return serialNumberPartern.SerialNumberPattern;
            return null;
        }
        public EffectivePartnerInvoiceSetting GetInvoicePartnerSetting(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
            Context.InvoicePartnerSettingsContext context = new Context.InvoicePartnerSettingsContext
            {
                PartnerId = partnerId,
                InvoiceTypeId = invoiceTypeId
            };
            return invoicePartnerManager.GetEffectivePartnerInvoiceSetting(context);
        }
        public bool CheckInvoiceFollowBillingPeriod(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
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
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
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
        public long GetInitialSequenceValue(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
            var partnerSettings = GetInvoicePartnerSetting(invoiceTypeId, partnerId);
            InvoicePartnerSettingPartContext invoicePartnerSettingPartContext = new InvoicePartnerSettingPartContext
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = partnerId,
                InvoiceSettingId = partnerSettings.InvoiceSetting.InvoiceSettingId
            };
            var initialSequenceValueSettingPart = invoicePartnerManager.GetInvoicePartnerSettingPart<InitialSequenceValueSettingPart>(invoicePartnerSettingPartContext);
            if (initialSequenceValueSettingPart != null)
                return initialSequenceValueSettingPart.InitialValue;
            return 1;
        }
    }
}
