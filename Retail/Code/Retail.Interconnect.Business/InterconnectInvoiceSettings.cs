﻿using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.Interconnect.Business
{
    public enum InterconnectInvoiceType { Customer = 0, Supplier = 1 }

    public class InterconnectInvoiceSettings : BaseRetailInvoiceTypeSettings
    {
        public Guid InvoiceTransactionTypeId { get; set; }
        public List<Guid> UsageTransactionTypeIds { get; set; }
        public InterconnectInvoiceType Type { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("D2776CD1-0900-4FF6-99BC-454866EAAD74"); }
        }

        public override dynamic GetInfo(Vanrise.Invoice.Entities.IInvoiceTypeExtendedSettingsInfoContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(this.AccountBEDefinitionId, context.Invoice.PartnerId);

            switch (context.InfoType)
            {
                case "MailTemplate":
                    long accountId = Convert.ToInt32(financialAccountData.Account.AccountId);
                    var account = accountBEManager.GetAccount(this.AccountBEDefinitionId, accountId);
                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                    objects.Add("Operator", account);
                    objects.Add("Invoice", context.Invoice);
                    return objects;
                case "BankDetails":
                    {
                        #region BankDetails
                        return accountBEManager.GetBankDetailsIds(this.AccountBEDefinitionId, financialAccountData.Account.AccountId);
                        #endregion
                    }
            }
            return null;
        }

        public override void GetInitialPeriodInfo(Vanrise.Invoice.Entities.IInitialPeriodInfoContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(this.AccountBEDefinitionId, context.PartnerId);
            context.PartnerCreationDate = financialAccountData.Account.CreatedTime;

        }

        public override Vanrise.Invoice.Entities.InvoiceGenerator GetInvoiceGenerator()
        {
            return new InterconnectInvoiceGenerator(this.AccountBEDefinitionId, this.InvoiceTransactionTypeId, this.UsageTransactionTypeIds, this.Type);
        }

        public override IEnumerable<string> GetPartnerIds(Vanrise.Invoice.Entities.IExtendedSettingsPartnerIdsContext context)
        {
            switch (context.PartnerRetrievalType)
            {
                case PartnerRetrievalType.GetActive:
                case PartnerRetrievalType.GetAll:
                    FinancialAccountManager financialAccountManager = new FinancialAccountManager();
                    return financialAccountManager.GetAllFinancialAccountsIds(this.AccountBEDefinitionId);
                default:
                    return null;
            }
        }

        public override Vanrise.Invoice.Entities.InvoicePartnerManager GetPartnerManager()
        {
            if (this.Type == InterconnectInvoiceType.Customer)
                return new InterconnectPartnerSettings(this.AccountBEDefinitionId, InterconnectPartnerType.Customer);

            else
                return new InterconnectPartnerSettings(this.AccountBEDefinitionId, InterconnectPartnerType.Supplier);
        }
    }
}
