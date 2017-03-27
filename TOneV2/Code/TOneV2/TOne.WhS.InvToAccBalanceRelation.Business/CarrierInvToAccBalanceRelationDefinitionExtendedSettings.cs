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
namespace TOne.WhS.InvToAccBalanceRelation.Business
{
    public class CarrierInvToAccBalanceRelationDefinitionExtendedSettings : InvToAccBalanceRelationDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("F5CD8367-A6DC-421E-B93C-0567ED769150"); }
        }

        public override List<InvoiceAccountInfo> GetBalanceInvoiceAccounts(IInvToAccBalanceRelGetBalanceInvoiceAccountsContext context)
        {

            List<InvoiceAccountInfo> invoiceAccountInfo = new List<InvoiceAccountInfo>();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var financialAccount = new FinancialAccountManager().GetFinancialAccount(Convert.ToInt32(context.AccountId));
            if(financialAccount == null)
                throw new NullReferenceException(string.Format("financialAccount: financialAccountId {0}",context.AccountId));
            var customerInvoiceTypeId = new Guid("EADC10C8-FFD7-4EE3-9501-0B2CE09029AD");

            if(financialAccount.CarrierAccountId.HasValue)
            {
                string partnerId = null;
                var carrierAccount = carrierAccountManager.GetCarrierAccount(financialAccount.CarrierAccountId.Value);
                carrierAccount.ThrowIfNull("carrierAccount", financialAccount.CarrierAccountId.Value);
                if (carrierAccount.AccountType == CarrierAccountType.Customer || carrierAccount.AccountType == CarrierAccountType.Exchange)
                {
                    var carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);
                    if (carrierProfile.Settings.CustomerInvoiceByProfile)
                    {
                        partnerId = string.Format("Profile_{0}", carrierAccount.CarrierProfileId);

                    }
                    else
                    {
                        partnerId = string.Format("Account_{0}", carrierAccount.CarrierAccountId);
                    }
                    invoiceAccountInfo.Add(new InvoiceAccountInfo
                    {
                        InvoiceTypeId = customerInvoiceTypeId,
                        PartnerId = partnerId
                    });
                }
               
            }else {
               var carrierProfile = carrierProfileManager.GetCarrierProfile(financialAccount.CarrierProfileId.Value);
               if (carrierProfile.Settings.CustomerInvoiceByProfile)
               {
                   string partnerId = string.Format("Profile_{0}", financialAccount.CarrierProfileId.Value);
                   invoiceAccountInfo.Add(new InvoiceAccountInfo
                   {
                       InvoiceTypeId = customerInvoiceTypeId,
                       PartnerId = partnerId
                   });
               }else
               {
                   var accounts = carrierAccountManager.GetCarriersByProfileId(financialAccount.CarrierProfileId.Value,true,false);
                   if (accounts != null && accounts.Count() > 0)
                   {
                       foreach (var account in accounts)
                       {
                           string partnerId = string.Format("Account_{0}", account.CarrierAccountId);
                           invoiceAccountInfo.Add(new InvoiceAccountInfo
                           {
                               InvoiceTypeId = customerInvoiceTypeId,
                               PartnerId = partnerId
                           });
                       }
                   }
               }
            }
            //AccountTypeManager accountTypeManager = new AccountTypeManager();
            ////AccountTypeSettings accountTypeSettings = accountTypeManager.GetAccountTypeSettings( context.AccountTypeId);
            ////if(accountTypeSettings.InvToAccBalanceRelationId.HasValue)
            ////{
            ////    var invToAccBalanceRelationSettings = new InvToAccBalanceRelationDefinitionManager().GetRelationExtendedSettings(accountTypeSettings.InvToAccBalanceRelationId.Value);
            ////    invToAccBalanceRelationSettings.
            ////}

            return invoiceAccountInfo;
        }

        public override List<BalanceAccountInfo> GetInvoiceBalanceAccounts(IInvToAccBalanceRelGetInvoiceBalanceAccountsContext context)
        {
            throw new NotImplementedException();

        }
    }
}
