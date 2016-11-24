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

namespace TOne.WhS.Invoice.Business.Extensions
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
        CustomerRegNo = 7,
        RegNo = 8,
        RegAddress = 9,
        Name = 10,
        VatID = 11
    }
    public class CarrierPartnerSettings : InvoicePartnerSettings
    {
        public InvoiceType InvoiceType { get; set; }
        public override string PartnerFilterSelector
        {
            get
            {
                switch (this.InvoiceType)
                {
                    case Extensions.InvoiceType.Customer: return "whs-invoice-carrier-customer-filter-selector";
                    case Extensions.InvoiceType.Supplier: return "";
                    default: return null;
                }
            }
        }
        public override string PartnerSelector
        {
            get
            {
                switch (this.InvoiceType)
                {
                    case Extensions.InvoiceType.Customer: return "whs-invoice-carrier-customer-selector";
                    case Extensions.InvoiceType.Supplier: return "whs-invoice-carrier-supplier-selector";
                    default: return null;
                }
            }
        }
        public bool UseMaskInfo { get; set; }
        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            switch (context.InfoType)
            {
                case "InvoiceRDLCReport":
                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                    string[] partner = context.PartnerId.Split('_');
                    CurrencyManager currencyManager = new CurrencyManager();
                    CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                    CarrierProfile carrierProfile = null;
                    string carrierName = null;
                    string currencySymbol = null;
                    CompanySetting companySetting = null;
                    int partnerId = Convert.ToInt32(partner[1]);
                    if (partner[0].Equals("Profile"))
                    {
                        companySetting = carrierProfileManager.GetCompanySetting(partnerId);
                        carrierProfile = carrierProfileManager.GetCarrierProfile(partnerId);
                        carrierName = carrierProfileManager.GetCarrierProfileName(carrierProfile.CarrierProfileId);
                        currencySymbol = currencyManager.GetCurrencySymbol(carrierProfile.Settings.CurrencyId);
                    }
                    else if (partner[0].Equals("Account"))
                    {
                        CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                        companySetting = carrierAccountManager.GetCompanySetting(partnerId);
                        var carrierAccount = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(partnerId));
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
                        if (carrierProfile.Settings.RegistrationNumber != null)
                        {
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.CustomerRegNo, carrierProfile.Settings.RegistrationNumber, true);
                        }
                    }
                    if (companySetting != null)
                    {
                        VRFileManager fileManager = new VRFileManager();
                        var logo = fileManager.GetFile(companySetting.CompanyLogo);
                        if (logo != null)
                        {
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.Image, Convert.ToBase64String(logo.Content), true);
                        }
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.RegNo, companySetting.RegistrationNumber, true);
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.RegAddress, companySetting.RegistrationAddress, true);
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Name, companySetting.CompanyName, true);
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.VatID, companySetting.VatId, true);

                    }
                    return rdlcReportParameters;
            }
            return null;
        }
        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            string[] partnerId = context.PartnerId.Split('_');
            if (partnerId[0].Equals("Profile"))
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
        public override dynamic GetActualPartnerId(IActualPartnerContext context)
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
        public override int GetPartnerDuePeriod(IPartnerDuePeriodContext context)
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
