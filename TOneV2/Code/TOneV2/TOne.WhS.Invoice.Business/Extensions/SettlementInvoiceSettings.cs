using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SettlementInvoiceSettings : InvoiceTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("C7A1C706-2C07-42CE-9728-E47BEE2A3129"); }
        }
        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new SettlementInvoiceGenerator(CustomerInvoiceTypeId, SupplierInvoiceTypeId);
        }
        public override GenerationCustomSection GenerationCustomSection
        {
            get { return new SettlementGenerationCustomSection(CustomerInvoiceTypeId, SupplierInvoiceTypeId); }
        }
        public override InvoicePartnerManager GetPartnerManager()
        {
            return new CarrierPartnerSettings();
        }
        public Guid InvoiceTransactionTypeId { get; set; }
        public Guid CustomerInvoiceTypeId { get; set; }
        public Guid SupplierInvoiceTypeId { get; set; }
        public List<Guid> UsageTransactionTypeIds { get; set; }
        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {

            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.Invoice.PartnerId));
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            
            switch(context.InfoType)
            {
                case "MailTemplate":
                    {
                        Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                        objects.Add("SupplierInvoice", context.Invoice);
                        CarrierProfile carrierProfile = null;
                        int carrierProfileId;
                        CompanySetting companySetting;
                        if (financialAccount.CarrierProfileId.HasValue)
                        {
                            carrierProfileId = financialAccount.CarrierProfileId.Value;
                            companySetting = carrierProfileManager.GetCompanySetting(financialAccount.CarrierProfileId.Value);
                        }
                        else
                        {
                            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                            var account = carrierAccountManager.GetCarrierAccount(financialAccount.CarrierAccountId.Value);
                            carrierProfileId = account.CarrierProfileId;
                            companySetting = carrierAccountManager.GetCompanySetting(financialAccount.CarrierAccountId.Value);
                        }
                        carrierProfile = carrierProfileManager.GetCarrierProfile(carrierProfileId);
                        if (carrierProfile != null)
                        {
                            objects.Add("Profile", carrierProfile);
                        }
                        objects.Add("CompanySetting", companySetting);
                        return objects;
                    }
                case "BankDetails":
                    {
                        #region BankDetails
                        IEnumerable<Guid> bankDetails = null;
                        if (financialAccount.CarrierProfileId.HasValue)
                        {
                            bankDetails = carrierProfileManager.GetBankDetails(financialAccount.CarrierProfileId.Value);
                        }
                        else
                        {
                             CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                             bankDetails = carrierAccountManager.GetBankDetails(financialAccount.CarrierAccountId.Value);
                        }
                        return bankDetails;
                        #endregion
                    }
            }
            return null;
        }
        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));
            CarrierProfile carrierProfile = null;
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            if (financialAccount.CarrierProfileId.HasValue)
            {
                carrierProfile = carrierProfileManager.GetCarrierProfile(Convert.ToInt32(financialAccount.CarrierProfileId.Value));
                context.PartnerCreationDate = carrierProfile.CreatedTime;
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                var account = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(financialAccount.CarrierAccountId.Value));
                context.PartnerCreationDate = account.CreatedTime;
            }
        }
        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            WHSFinancialAccountInfoFilter invoiceAccountInfoFilter = new WHSFinancialAccountInfoFilter { InvoiceTypeId = context.InvoiceTypeId };
            switch (context.PartnerRetrievalType)
            {
                case PartnerRetrievalType.GetAll:
                  break;
                case PartnerRetrievalType.GetActive:
                  invoiceAccountInfoFilter.Status = VRAccountStatus.Active;
                  break;
                case PartnerRetrievalType.GetInactive:
                  invoiceAccountInfoFilter.Status = VRAccountStatus.InActive;
                    break;
                default : throw new NotSupportedException(string.Format("PartnerRetrievalType {0} not supported.",context.PartnerRetrievalType));
            }
            var carriers = new WHSFinancialAccountManager().GetFinancialAccountsInfo(invoiceAccountInfoFilter);
             if (carriers == null)
                        return null;
            return carriers.Select(x => x.FinancialAccountId.ToString());
        }
    }
}
