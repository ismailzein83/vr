using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.MultiNet.Business
{
    public class MultiNetSubscriberInvoiceSettings : InvoiceTypeExtendedSettings
    {
        public Guid AccountBEDefinitionId { get; set; }
        public Guid CompanyExtendedInfoPartdefinitionId { get; set; }
        public List<Guid> SalesTaxChargeableEntities { get; set; }
        public List<Guid> WHTaxChargeableEntities { get; set; }
        public Guid InComingChargeableEntity { get; set; }
        public Guid OutGoingChargeableEntity { get; set; }
        public Guid SalesTaxRuleDefinitionId { get; set; }
        public Guid WHTaxRuleDefinitionId { get; set; }
        public Guid latePaymentRuleDefinitionId { get; set; }
        
        public override Guid ConfigId
        {
            get { return new Guid("3C121314-56F2-445D-91C7-896D07D09BDD"); }
        }

        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {
            switch (context.InfoType)
            {
                case "MailTemplate":
                    FinancialAccountManager financialAccountManager = new FinancialAccountManager();
                    var financialAccountData = financialAccountManager.GetFinancialAccountData(this.AccountBEDefinitionId, context.Invoice.PartnerId);
                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                    objects.Add("Account", financialAccountData.Account);
                    objects.Add("Invoice", context.Invoice);
                    return objects;
            }
            return null;
        }

        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(this.AccountBEDefinitionId, context.PartnerId);
            context.PartnerCreationDate = financialAccountData.Account.CreatedTime;

        }

        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new MultiNetSubscriberInvoiceGenerator(this.AccountBEDefinitionId, this.SalesTaxChargeableEntities, this.WHTaxChargeableEntities, this.InComingChargeableEntity, this.OutGoingChargeableEntity, this.SalesTaxRuleDefinitionId, this.WHTaxRuleDefinitionId, this.latePaymentRuleDefinitionId);
        }

        public override InvoicePartnerManager GetPartnerManager()
        {
            return new MultiNetSubscriberPartnerSettings(this.AccountBEDefinitionId);
        }

        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
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
    }
}
