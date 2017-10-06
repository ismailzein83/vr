using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Invoice.Business.Context;

namespace Vanrise.Invoice.MainExtensions.PartnerGroup
{
    public enum InvoicePartnerSetting { All = 0, OnlyEnabledAutomaticInvoice = 1, OnlyDisabledAutomaticInvoice = 2 }

    public class InvoiceSetting : Vanrise.Invoice.Entities.PartnerGroup
    {
        public override Guid ConfigId { get { return new Guid("CF988AC3-FF1A-49F7-9293-4E7FD1E8E270"); } }
        public Guid InvoiceSettingId { get; set; }
        public InvoicePartnerSetting Setting { get; set; }

        public override List<string> GetPartnerIds(IPartnerGroupContext context)
        {
            PartnerManager partnerManager = new PartnerManager();
            InvoicePartnerManager invoicePartnerManager = partnerManager.GetPartnerManager(context.InvoiceTypeId);

            var invoiceType = new InvoiceTypeManager().GetInvoiceType(context.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", context.InvoiceTypeId);

            var partnerIds = invoiceType.Settings.ExtendedSettings.GetPartnerIds(new ExtendedSettingsPartnerIdsContext { InvoiceTypeId = context.InvoiceTypeId, PartnerRetrievalType = Entities.PartnerRetrievalType.GetAll });


            if (partnerIds == null)
                return null;

            List<string> invoiceSettingPartnerIds = new List<string>();

            DateTime now = DateTime.Now;

            foreach (string partnerId in partnerIds)
            {
                var partnerSetting = partnerManager.GetInvoicePartnerSetting(context.InvoiceTypeId, partnerId);
                if (partnerSetting == null || partnerSetting.InvoiceSetting == null || partnerSetting.InvoiceSetting.InvoiceSettingId != this.InvoiceSettingId)
                    continue;

                bool isInvoicePartnerSettingMatching = false;
                AutomaticInvoiceSettingPart automaticInvoiceSettingPart = invoicePartnerManager.GetInvoicePartnerSettingPart<AutomaticInvoiceSettingPart>(new InvoicePartnerSettingPartContext() { InvoiceSettingId = this.InvoiceSettingId, InvoiceTypeId = context.InvoiceTypeId, PartnerId = partnerId });

                switch (this.Setting)
                {
                    case InvoicePartnerSetting.All: isInvoicePartnerSettingMatching = true; break;
                    case InvoicePartnerSetting.OnlyEnabledAutomaticInvoice: if (automaticInvoiceSettingPart.IsEnabled) { isInvoicePartnerSettingMatching = true; } break;
                    case InvoicePartnerSetting.OnlyDisabledAutomaticInvoice: if (!automaticInvoiceSettingPart.IsEnabled) { isInvoicePartnerSettingMatching = true; } break;
                    default: throw new NotSupportedException(string.Format("InvoicePartnerSetting '{0}' is not supported", this.Setting));
                }

                if (!isInvoicePartnerSettingMatching)
                    continue;

                VRInvoiceAccountData invoiceAccountData = partnerManager.GetInvoiceAccountData(context.InvoiceTypeId, partnerId);
                invoiceAccountData.ThrowIfNull("invoiceAccountData");

                DateTime? effectiveDate = context.EffectiveDate;
                bool? isEffectiveInFuture = context.IsEffectiveInFuture;
                DateTime? bed = invoiceAccountData.BED;
                DateTime? eed = invoiceAccountData.EED;

                if (invoiceAccountData.Status == context.Status
                    && (!effectiveDate.HasValue || ((!bed.HasValue || bed <= effectiveDate) && (!eed.HasValue || eed > effectiveDate)))
                    && (!isEffectiveInFuture.HasValue || (isEffectiveInFuture.Value && (!eed.HasValue || eed >= now)) || (!isEffectiveInFuture.Value && eed.HasValue && eed <= now)))
                {
                    invoiceSettingPartnerIds.Add(partnerId);
                }
            }

            return invoiceSettingPartnerIds.Count > 0 ? invoiceSettingPartnerIds : null;
        }
    }
}