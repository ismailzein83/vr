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
        VatID = 11,
        CustomerVatID = 12
    }
    public class CarrierPartnerSettings : InvoicePartnerSettings
    {
        public override string PartnerFilterSelector
        {
            get
            {
                return "whs-invoice-carrier-customer-filter-selector";
            }
        }
        public override string PartnerSelector
        {
            get
            {
                return "whs-invoice-carrier-customer-selector";
            }
        }
        public bool UseMaskInfo { get; set; }
        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            string[] partner = context.PartnerId.Split('_');
            int partnerId = Convert.ToInt32(partner[1]);
            switch (context.InfoType)
            {
                case "InvoiceRDLCReport":
                    {
                        #region InvoiceRDLCReport
                        Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                        CurrencyManager currencyManager = new CurrencyManager();
                        CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                        CarrierProfile carrierProfile = null;
                        string carrierName = null;
                        string currencySymbol = null;
                        CompanySetting companySetting = null;
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
                            if (carrierProfile.Settings.TaxSetting != null && carrierProfile.Settings.TaxSetting.VATId != null)
                            {
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.CustomerVatID, carrierProfile.Settings.TaxSetting.VATId, true);
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
                        #endregion
                    }
                
                    
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

            if (partnerId[0].Equals("Profile"))
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();

                return carrierProfileManager.GetDuePeriod(Convert.ToInt32(partnerId[1]));
            }
            else if (partnerId[0].Equals("Account"))
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                return carrierAccountManager.GetDuePeriod(Convert.ToInt32(partnerId[1]));
            }
            return 0;
        }
        public override bool CheckInvoiceFollowBillingPeriod(ICheckInvoiceFollowBillingPeriodContext context)
        {
            string[] partnerId = context.PartnerId.Split('_');

            if (partnerId[0].Equals("Profile"))
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();

                return carrierProfileManager.CheckInvoiceFollowBillingPeriod(Convert.ToInt32(partnerId[1]));
            }
            else if (partnerId[0].Equals("Account"))
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                return carrierAccountManager.CheckInvoiceFollowBillingPeriod(Convert.ToInt32(partnerId[1]));
            }
            return false;
        }
        private void AddRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter key, string value, bool isVisible)
        {
            if (rdlcReportParameters == null)
                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }
        public override string GetPartnerSerialNumberPattern(IPartnerSerialNumberPatternContext context)
        {
            string[] partnerId = context.PartnerId.Split('_');

            if (partnerId[0].Equals("Profile"))
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                return carrierProfileManager.GetInvoiceSerialNumberPattern(Convert.ToInt32(partnerId[1]));
            }
            else if (partnerId[0].Equals("Account"))
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                return carrierAccountManager.GetInvoiceSerialNumberPattern(Convert.ToInt32(partnerId[1]));
            }
            return null;
        }

    }
      
}
