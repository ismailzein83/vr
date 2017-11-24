using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
namespace Vanrise.Invoice.Business
{
    public class PartnerManager
    {
        public InvoicePartnerManager GetPartnerManager(Guid invoiceTypeId)
        {
            InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
            if (invoiceType.Settings == null)
                throw new NullReferenceException("invoiceType.Settings");
            if (invoiceType.Settings.ExtendedSettings == null)
                throw new NullReferenceException("invoiceType.Settings.ExtendedSettings");
            var invoicePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
            if (invoicePartnerManager == null)
                throw new NullReferenceException("invoicePartnerManager");
            return invoicePartnerManager;
        }
        public string GetPartnerName(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);

            IPartnerNameManagerContext partnerNameManagerContext = new PartnerNameManagerContext
            {
                PartnerId = partnerId
            };
            return invoicePartnerManager.GetPartnerName(partnerNameManagerContext);
        }
        public string GetPartnerInvoiceSettingFilterFQTN(Guid invoiceTypeId)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
            return invoicePartnerManager.PartnerInvoiceSettingFilterFQTN;
        }
        public dynamic GetPartnerInfo(Guid invoiceTypeId, string partnerId, string infoType)
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

        public string EvaluateInvoiceFileNamePattern(Guid invoiceTypeId, string partnerId, Entities.Invoice invoice)
        {
            var fileNamePattern = GetPartnerFileNamePattern(invoiceTypeId, partnerId);
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
            if (invoiceType.Settings.InvoiceFileSettings != null)
            {
                if (invoiceType.Settings.InvoiceFileSettings.InvoiceFileNameParts != null)
                {
                    InvoiceFileNamePartContext invoiceFileNamePartContext = new InvoiceFileNamePartContext
                    {
                        Invoice = invoice,
                        InvoiceTypeId = invoiceTypeId
                    };
                    foreach (var part in invoiceType.Settings.InvoiceFileSettings.InvoiceFileNameParts)
                    {
                        if (fileNamePattern != null && fileNamePattern.Contains(string.Format("#{0}#", part.VariableName)))
                        {
                            fileNamePattern = fileNamePattern.Replace(string.Format("#{0}#", part.VariableName), part.Settings.GetPartText(invoiceFileNamePartContext));
                        }
                    }
                }
            }
            if (fileNamePattern == null)
            {
                return "Invoice";
            }
            return fileNamePattern;
        }

        public T GetInvoicePartnerSettingPart<T>(Guid invoiceTypeId, string partnerId) where T : InvoiceSettingPart
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
            var partnerSettings = GetInvoicePartnerSetting(invoiceTypeId, partnerId);
            InvoicePartnerSettingPartContext invoicePartnerSettingPartContext = new InvoicePartnerSettingPartContext
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = partnerId,
                InvoiceSettingId = partnerSettings.InvoiceSetting.InvoiceSettingId
            };
            return invoicePartnerManager.GetInvoicePartnerSettingPart<T>(invoicePartnerSettingPartContext) as T;
        }
        public string GetPartnerFileNamePattern(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
            var partnerSettings = GetInvoicePartnerSetting(invoiceTypeId, partnerId);
            InvoicePartnerSettingPartContext invoicePartnerSettingPartContext = new InvoicePartnerSettingPartContext
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = partnerId,
                InvoiceSettingId = partnerSettings.InvoiceSetting.InvoiceSettingId
            };
            var fileNamePartner = invoicePartnerManager.GetInvoicePartnerSettingPart<FileNamePatternInvoiceSettingPart>(invoicePartnerSettingPartContext) as FileNamePatternInvoiceSettingPart;
            if (fileNamePartner != null)
                return fileNamePartner.FileNamePattern;
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
        public VRInvoiceAccountData GetInvoiceAccountData(Guid invoiceTypeId, string partnerId)
        {
            var invoicePartnerManager = GetPartnerManager(invoiceTypeId);
            var partnerSettings = GetInvoicePartnerSetting(invoiceTypeId, partnerId);
            InvoiceAccountDataContext invoiceAccountDataContext = new InvoiceAccountDataContext
            {
                PartnerId = partnerId,
                InvoiceTypeId = invoiceTypeId
            };
            return invoicePartnerManager.GetInvoiceAccountData(invoiceAccountDataContext);
        }
    }
}
