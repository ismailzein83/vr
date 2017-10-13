﻿using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Invoice.Entities;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using System.Collections.Generic;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SupplierGenerationCustomSection : GenerationCustomSection
    {
        public override string GenerationCustomSectionDirective
        {
            get
            {
                return "whs-invoicetype-generationcustomsection-supplier";
            }
        }

        public override dynamic GetGenerationCustomPayload(IGetGenerationCustomPayloadContext context)
        {
            int? timeZoneId;
            decimal? commission = null;
            CommissionType commissionType = CommissionType.Display;

            WHSFinancialAccountManager whsFinancialAccountManager = new WHSFinancialAccountManager();

            var financialAccount = whsFinancialAccountManager.GetFinancialAccount(int.Parse(context.PartnerId));
            financialAccount.ThrowIfNull("financialAccount", context.PartnerId);
            financialAccount.Settings.ThrowIfNull("financialAccount.Settings", context.PartnerId);

            if (financialAccount.CarrierAccountId.HasValue)
            {
                timeZoneId = new CarrierAccountManager().GetSupplierTimeZoneId(financialAccount.CarrierAccountId.Value);
            }
            else
            {
                timeZoneId = new CarrierProfileManager().GetSupplierTimeZoneId(financialAccount.CarrierProfileId.Value);
            }
            List<FinancialAccountInvoiceSetting> financialAccountInvoiceSettings = financialAccount.Settings.FinancialAccountInvoiceSettings;
            if (financialAccountInvoiceSettings != null && financialAccountInvoiceSettings.Count > 0)
            {
                FinancialAccountInvoiceSetting financialAccountInvoiceSetting = financialAccountInvoiceSettings.FindRecord(itm => itm.InvoiceTypeId == context.InvoiceTypeId);
                if (financialAccountInvoiceSetting != null && financialAccountInvoiceSetting.FinancialAccountCommission != null)
                {
                    commission = financialAccountInvoiceSetting.FinancialAccountCommission.Commission;
                    commissionType = financialAccountInvoiceSetting.FinancialAccountCommission.CommissionType;
                }
            }

            return new SupplierGenerationCustomSectionPayload() { TimeZoneId = timeZoneId, Commission = commission, CommissionType = commissionType };
        }
    }
}
