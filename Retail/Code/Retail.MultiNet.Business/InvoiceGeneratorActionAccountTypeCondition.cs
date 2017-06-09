using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
namespace Retail.MultiNet.Business
{
    public class InvoiceGeneratorActionAccountTypeCondition : PartnerInvoiceFilterCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("E557711B-8164-4880-9482-7FBCB99EB8A4"); }
        }
        public Guid AccountTypeId { get; set; }
        public override bool IsFilterMatch(IPartnerInvoiceFilterConditionContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var invoiceTypeExtendedSettings = context.InvoiceType.Settings.ExtendedSettings.CastWithValidate<MultiNetSubscriberInvoiceSettings>("InvoiceType.Settings.ExtendedSettings");
            var financialAccountData = financialAccountManager.GetFinancialAccountData(invoiceTypeExtendedSettings.AccountBEDefinitionId,context.generateInvoiceInput.PartnerId);
            financialAccountData.ThrowIfNull("financialAccountData");
            if (financialAccountData.Account.TypeId == this.AccountTypeId)
                return true;
            return false;
        }
    }
}
