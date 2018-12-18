//using Retail.BusinessEntity.Business;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Invoice.Entities;

//namespace Retail.Demo.Business
//{
//    public enum SMSInvoiceType { Customer = 0, Supplier = 1 }

//    public class SMSInvoiceSettings : BaseRetailInvoiceTypeSettings
//    {
//        public SMSInvoiceType Type { get; set; }
//        public override Guid ConfigId
//        {
//            get { return new Guid("bb80cf36-b7f3-4933-876b-e9d982fe27df"); }
//        }
//        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
//        {
//            AccountBEManager accountBEManager = new AccountBEManager();
//            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
//            var financialAccountData = financialAccountManager.GetFinancialAccountData(this.AccountBEDefinitionId, context.Invoice.PartnerId);

//            switch (context.InfoType)
//            {
//                case "MailTemplate":
//                    long accountId = Convert.ToInt32(financialAccountData.Account.AccountId);
//                    var account = accountBEManager.GetAccount(this.AccountBEDefinitionId, accountId);
//                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
//                    objects.Add("Operator", account);
//                    objects.Add("Invoice", context.Invoice);
//                    return objects;
//                case "BankDetails":
//                    {
//                        #region BankDetails
//                        return accountBEManager.GetBankDetailsIds(financialAccountData.Account.AccountId);
//                        #endregion
//                    }
//            }
//            return null;
//        }

//        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
//        {
//            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
//            var financialAccountData = financialAccountManager.GetFinancialAccountData(this.AccountBEDefinitionId, context.PartnerId);
//            context.PartnerCreationDate = financialAccountData.Account.CreatedTime;
//        }

//        public override InvoiceGenerator GetInvoiceGenerator()
//        {
//            return new SMSInvoiceGenerator(this.AccountBEDefinitionId, this.Type);
//        }

//        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
//        {
//            switch (context.PartnerRetrievalType)
//            {
//                case PartnerRetrievalType.GetActive:
//                case PartnerRetrievalType.GetAll:
//                    FinancialAccountManager financialAccountManager = new FinancialAccountManager();
//                    return financialAccountManager.GetAllFinancialAccountsIds(this.AccountBEDefinitionId);
//                default:
//                    return null;
//            }
//        }

//        public override InvoicePartnerManager GetPartnerManager()
//        {
//            return new SMSInvoicePartnerSettings(this.AccountBEDefinitionId);
//        }
//    }
//}
