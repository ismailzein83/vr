using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business
{
    public class CarrierPartnerManager : IPartnerManager
    {
        public enum RDLCParameter 
        { 
            Customer = 0,
            Currency = 1,
            Address = 2,
            Phone = 3,
            Phone1 = 4,
            Fax = 5,
            Image = 6,
            CustomerRegNo = 7
            
        }
        public dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            switch(context.InfoType)
            {
                case "InvoiceRDLCReport":
                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                     string[] partnerId = context.PartnerId.Split('_');
                     CurrencyManager currencyManager = new CurrencyManager();
                     CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                     CarrierProfile carrierProfile = null;
                     string carrierName = null;
                     string currencySymbol = null;

                     if (partnerId[0].Equals("Profile"))
                     {
                         carrierProfile = carrierProfileManager.GetCarrierProfile(Convert.ToInt32(partnerId[1]));
                         carrierName = carrierProfileManager.GetCarrierProfileName(carrierProfile.CarrierProfileId);
                         currencySymbol = currencyManager.GetCurrencySymbol(carrierProfile.Settings.CurrencyId);
                     }
                     else if (partnerId[0].Equals("Account"))
                     {
                         CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                         var carrierAccount = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(partnerId[1]));
                         carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);
                         carrierName = carrierAccountManager.GetCarrierAccountName(carrierAccount.CarrierAccountId);
                         currencySymbol = currencyManager.GetCurrencySymbol(carrierAccount.CarrierAccountSettings.CurrencyId);
                     }
                   
                     AddRDLCParameter(rdlcReportParameters, RDLCParameter.Customer, carrierName, true);
                     AddRDLCParameter(rdlcReportParameters, RDLCParameter.Currency, currencySymbol, true);
                     if (carrierProfile != null)
                     {
                         if (carrierProfile.Settings.Address != null)
                             AddRDLCParameter(rdlcReportParameters, RDLCParameter.Address, carrierProfile.Settings.Address, true);
                         if (carrierProfile.Settings.PhoneNumbers != null)
                         {
                             AddRDLCParameter(rdlcReportParameters, RDLCParameter.Phone, carrierProfile.Settings.PhoneNumbers.ElementAtOrDefault(0), true);
                             AddRDLCParameter(rdlcReportParameters, RDLCParameter.Phone1, carrierProfile.Settings.PhoneNumbers.ElementAtOrDefault(1), true);
                         }
                         if (carrierProfile.Settings.Faxes != null)
                         {
                             AddRDLCParameter(rdlcReportParameters, RDLCParameter.Fax, carrierProfile.Settings.Faxes.ElementAtOrDefault(0), true);
                         }
                         if (carrierProfile.Settings.CompanyLogo != null)
                         {
                             VRFileManager fileManager = new VRFileManager();
                             var logo = fileManager.GetFile(carrierProfile.Settings.CompanyLogo);
                             AddRDLCParameter(rdlcReportParameters, RDLCParameter.Image, Convert.ToBase64String(logo.Content), true);
                         }
                         if(carrierProfile.Settings.RegistrationNumber != null)
                         {
                             AddRDLCParameter(rdlcReportParameters, RDLCParameter.CustomerRegNo, carrierProfile.Settings.RegistrationNumber, true);

                         }
                     }

                    return rdlcReportParameters;
            }
            return null;
        }
        public string GetPartnerName(IPartnerManagerContext context)
        {
            string[] partnerId = context.PartnerId.Split('_');
            if(partnerId[0].Equals("Profile"))
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                return carrierProfileManager.GetCarrierProfileName(Convert.ToInt32(partnerId[1]));
            }
            else if (partnerId[0].Equals("Account"))
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                return carrierAccountManager.GetCarrierAccountName(Convert.ToInt32(partnerId[1]));
            }
            return null;
        }
        public dynamic GetActualPartnerId(IPartnerManagerContext context)
        {
            string[] partnerId = context.PartnerId.Split('_');
            if (partnerId[0].Equals("Profile"))
            {
                return Convert.ToInt32(partnerId[1]);
            }
            else if (partnerId[0].Equals("Account"))
            {
                return Convert.ToInt32(partnerId[1]);
            }
            return null;
        }
        public int GetPartnerDuePeriod(IPartnerManagerContext context)
        {
            string[] partnerId = context.PartnerId.Split('_');
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();

            if (partnerId[0].Equals("Profile"))
            {
                var carrierProfile = carrierProfileManager.GetCarrierProfile(Convert.ToInt32(partnerId[1]));
                return carrierProfile.Settings.DuePeriod;
            }
            else if (partnerId[0].Equals("Account"))
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                var carrierAccount = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(partnerId[1]));
                var carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);
                return carrierProfile.Settings.DuePeriod;
            }
            return 0;
        }


        private void AddRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter key, string value, bool isVisible)
        {
            if (rdlcReportParameters == null)
                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }
    }
}
