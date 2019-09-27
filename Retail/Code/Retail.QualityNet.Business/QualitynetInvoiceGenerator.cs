using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Invoice.Entities;

namespace Retail.QualityNet.Business
{
    public class QualitynetInvoiceGenerator : InvoiceGenerator
    {
        Guid _acountBEDefinitionId { get; set; }
        public QualitynetInvoiceGenerator(Guid acountBEDefinitionId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
        }

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            FinancialAccountData financialAccountData = new FinancialAccountManager().GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            if (context.FromDate < financialAccountData.FinancialAccount.BED || context.ToDate > financialAccountData.FinancialAccount.EED)
            {
                context.ErrorMessage = "From date and To date should be within the effective date of financial account.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                return;
            }

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet();
        }

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet()
        {
            throw new NotImplementedException();
            //List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

            //GeneratedInvoiceItemSet invoiceItemSet = new GeneratedInvoiceItemSet()
            //{
            //    SetName = "InvoiceItems",
            //    Items = new List<GeneratedInvoiceItem>()
            //};

            //invoiceItemSet.Items.Add(new GeneratedInvoiceItem
            //{
            //    Name = " ",
            //    Details = new
            //    {
            //        Caller = ,
            //        PhoneNumber = ,
            //        Country = ,

            //    }
            //});

            //generatedInvoiceItemSets.Add(invoiceItemSet);

            //return generatedInvoiceItemSets;
        }
    }
}