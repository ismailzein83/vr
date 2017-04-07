using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.InvToAccBalanceRelation.Business;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.Invoice.Business;
namespace TOne.WhS.InvToAccBalanceRelation.Business
{
    public class CarrierInvToAccBalanceRelationDefinitionExtendedSettings : InvToAccBalanceRelationDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("F5CD8367-A6DC-421E-B93C-0567ED769150"); }
        }

        public override List<Vanrise.InvToAccBalanceRelation.Entities.InvoiceAccountInfo> GetBalanceInvoiceAccounts(IInvToAccBalanceRelGetBalanceInvoiceAccountsContext context)
        {

            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccount = new FinancialAccountManager().GetFinancialAccount(Convert.ToInt32(context.AccountId));
            if (financialAccount == null)
                throw new NullReferenceException(string.Format("financialAccount: financialAccountId {0}", context.AccountId));
            List<Vanrise.InvToAccBalanceRelation.Entities.InvoiceAccountInfo> invoiceAccountInfo = new List<Vanrise.InvToAccBalanceRelation.Entities.InvoiceAccountInfo>();

            FinancialAccountDefinitionManager financialAccountDefinitionManager = new AccountBalance.Business.FinancialAccountDefinitionManager();
            var accountBalanceSettings = financialAccountDefinitionManager.GetFinancialAccountDefinitionExtendedSettings<AccountBalanceSettings>(context.AccountTypeId);
           
            InvoiceAccountManager invoiceAccountManager = new InvoiceAccountManager();
            if(accountBalanceSettings.IsApplicableToCustomer)
            {
                if(financialAccount.CarrierProfileId.HasValue)
                {
                    List<CarrierInvoiceAccountData> carrierInvoiceAccountsData;
                    if (invoiceAccountManager.TryGetCustProfInvoiceAccountData(financialAccount.CarrierProfileId.Value, context.EffectiveOn, out carrierInvoiceAccountsData))
                    {
                        AddInvoiceAccountInfo(invoiceAccountInfo, carrierInvoiceAccountsData);
                    }
                }else
                {
                    CarrierInvoiceAccountData carrierInvoiceAccountData;
                    if (invoiceAccountManager.TryGetCustAccInvoiceAccountData(financialAccount.CarrierAccountId.Value, context.EffectiveOn, out carrierInvoiceAccountData))
                    {
                        AddInvoiceAccountInfo(invoiceAccountInfo, carrierInvoiceAccountData);
                    }
                }
            }
            if(accountBalanceSettings.IsApplicableToSupplier)
            {
                if (financialAccount.CarrierProfileId.HasValue)
                {
                    List<CarrierInvoiceAccountData> carrierInvoiceAccountsData;
                    if (invoiceAccountManager.TryGetSuppProfInvoiceAccountData(financialAccount.CarrierProfileId.Value, context.EffectiveOn, out carrierInvoiceAccountsData))
                    {
                      AddInvoiceAccountInfo(invoiceAccountInfo,carrierInvoiceAccountsData);
                    }
                }
                else
                {
                    CarrierInvoiceAccountData carrierInvoiceAccountData;
                    if (invoiceAccountManager.TryGetSuppAccInvoiceAccountData(financialAccount.CarrierAccountId.Value, context.EffectiveOn, out carrierInvoiceAccountData))
                    {
                        AddInvoiceAccountInfo(invoiceAccountInfo,carrierInvoiceAccountData);
                    }
                }
            }
          //  var customerInvoiceTypeId = new Guid("EADC10C8-FFD7-4EE3-9501-0B2CE09029AD");
            return invoiceAccountInfo;
        }

        public override List<BalanceAccountInfo> GetInvoiceBalanceAccounts(IInvToAccBalanceRelGetInvoiceBalanceAccountsContext context)
        {
            throw new NotImplementedException();

        }

        private void AddInvoiceAccountInfo(List<Vanrise.InvToAccBalanceRelation.Entities.InvoiceAccountInfo> invoiceAccountInfo , CarrierInvoiceAccountData carrierInvoiceAccountData)
        {
            var partnerId = carrierInvoiceAccountData.InvoiceAccountId.ToString();
            if(!invoiceAccountInfo.Any(x=>x.InvoiceTypeId == carrierInvoiceAccountData.InvoiceTypeId && x.PartnerId == partnerId))
            {
               invoiceAccountInfo.Add(CreateInvoiceAccountInfo(carrierInvoiceAccountData.InvoiceTypeId,partnerId));
            }
        }
        private void AddInvoiceAccountInfo(List<Vanrise.InvToAccBalanceRelation.Entities.InvoiceAccountInfo> invoiceAccountInfo, List<CarrierInvoiceAccountData> carrierInvoiceAccountsData)
        {
            foreach (var carrierInvoiceAccountData in carrierInvoiceAccountsData)
            {
                AddInvoiceAccountInfo(invoiceAccountInfo, carrierInvoiceAccountData);
            }
        }
        private Vanrise.InvToAccBalanceRelation.Entities.InvoiceAccountInfo CreateInvoiceAccountInfo(Guid invoiceTypeId, string partnerId)
        {
            return new Vanrise.InvToAccBalanceRelation.Entities.InvoiceAccountInfo
            {
                InvoiceTypeId = invoiceTypeId,
                PartnerId = partnerId
            };
        }
    }
}
