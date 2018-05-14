using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.Interconnect.Business
{
    public enum RDLCParameter
    {
        AccountName = 0,
        Image = 1,
        RegNo = 2,
        RegAddress = 3,
        Name = 4,
        VatID = 5,
        Currency = 6,
        Address = 7,
        Phone = 8,
        Fax = 9,
        CompanyName = 10,
        Email = 11,
        NTN = 12,
        AccountType = 13,
        AccountNumber = 14,
        Attn = 15,
        AssignedNumber = 16
    }

    public class InterconnectPartnerSettings : InvoicePartnerManager
    {
        Guid _acountBEDefinitionId;
        public InterconnectPartnerSettings(Guid acountBEDefinitionId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
        }

        public override string PartnerFilterSelector
        {
            get
            {
                return "retail-interconnect-account-invoice-selector";
            }
        }
        public override string PartnerSelector
        {
            get
            {
                return "retail-interconnect-account-invoice-selector";
            }
        }
        public override string PartnerInvoiceSettingFilterFQTN
        {
            get
            {
                return "Retail.Interconnect.Business.AssignedFinancialAccountToInvoiceSettingFilter, Retail.Interconnect.Business";
            }
        }
        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            return financialAccountData.Account.AccountId;
        }

        public override VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            AccountBEManager accountBEManager = new AccountBEManager();
            return new VRInvoiceAccountData
            {
                BED = financialAccountData.FinancialAccount.BED,
                EED = financialAccountData.FinancialAccount.EED,
                Status = accountBEManager.IsAccountInvoiceActive(financialAccountData.Account) ? VRAccountStatus.Active : VRAccountStatus.InActive
            };
        }

        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            AccountBEManager accountBEManager = new AccountBEManager();
            return accountBEManager.GetAccountName(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
        }
    }
}
