using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Invoice.Entities;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using System.Collections.Generic;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CustomerGenerationCustomSection : GenerationCustomSection
    {
        public override string GenerationCustomSectionDirective
        {
            get
            {
                return "whs-invoicetype-generationcustomsection-customer";
            }

        }

        public override void EvaluateGenerationCustomPayload(IGetGenerationCustomPayloadContext context)
        {
            if(context.InvoiceGenerationInfo != null)
            {
                foreach(var invoiceGenerationInfo in context.InvoiceGenerationInfo)
                {
                    int? timeZoneId;
                    decimal? commission = null;
                    CommissionType commissionType = CommissionType.Display;

                    WHSFinancialAccountManager whsFinancialAccountManager = new WHSFinancialAccountManager();

                    var financialAccount = whsFinancialAccountManager.GetFinancialAccount(int.Parse(invoiceGenerationInfo.PartnerId));
                    financialAccount.ThrowIfNull("financialAccount", invoiceGenerationInfo.PartnerId);
                    financialAccount.Settings.ThrowIfNull("financialAccount.Settings", invoiceGenerationInfo.PartnerId);

                    if (financialAccount.CarrierAccountId.HasValue)
                    {
                        timeZoneId = new CarrierAccountManager().GetCustomerTimeZoneId(financialAccount.CarrierAccountId.Value);
                    }
                    else
                    {
                        timeZoneId = new CarrierProfileManager().GetCustomerTimeZoneId(financialAccount.CarrierProfileId.Value);
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
                    invoiceGenerationInfo.CustomPayload = new CustomerGenerationCustomSectionPayload() { TimeZoneId = timeZoneId, Commission = commission, CommissionType = commissionType };
                }
            }
           
        }
    }
}
