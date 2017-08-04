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
        Carrier = 0,
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
    public class CarrierPartnerSettings : InvoicePartnerManager
    {
        public override string PartnerFilterSelector
        {
            get
            {
                return "whs-invoice-account-selector";
            }
        }
        public override string PartnerSelector
        {
            get
            {
                return "whs-invoice-account-selector";
            }
        }
        public bool UseMaskInfo { get; set; }
        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            InvoiceAccountManager invoiceAccountManager = new Business.InvoiceAccountManager();
            var invoiceAccount = invoiceAccountManager.GetInvoiceAccount(Convert.ToInt32(context.PartnerId));
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

                        if (invoiceAccount.CarrierProfileId.HasValue)
                        {
                            companySetting = carrierProfileManager.GetCompanySetting(invoiceAccount.CarrierProfileId.Value);
                            carrierProfile = carrierProfileManager.GetCarrierProfile(invoiceAccount.CarrierProfileId.Value);
                            carrierName = carrierProfileManager.GetCarrierProfileName(carrierProfile.CarrierProfileId);
                            currencySymbol = currencyManager.GetCurrencySymbol(carrierProfile.Settings.CurrencyId);
                        }
                        else
                        {
                            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                            companySetting = carrierAccountManager.GetCompanySetting(invoiceAccount.CarrierAccountId.Value);
                            var carrierAccount = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(invoiceAccount.CarrierAccountId.Value));
                            carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);
                            carrierName = carrierAccountManager.GetCarrierAccountName(carrierAccount.CarrierAccountId);
                            currencySymbol = currencyManager.GetCurrencySymbol(carrierAccount.CarrierAccountSettings.CurrencyId);
                        }

                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Carrier, carrierName, true);
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
            InvoiceAccountManager invoiceAccountManager = new Business.InvoiceAccountManager();
            var invoiceAccount = invoiceAccountManager.GetInvoiceAccount(Convert.ToInt32(context.PartnerId));
            if (invoiceAccount.CarrierProfileId.HasValue)
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                return carrierProfileManager.GetCarrierProfileName(invoiceAccount.CarrierProfileId.Value);
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                return carrierAccountManager.GetCarrierAccountName(invoiceAccount.CarrierAccountId.Value);
            }
        }
        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            InvoiceAccountManager invoiceAccountManager = new Business.InvoiceAccountManager();
            var invoiceAccount = invoiceAccountManager.GetInvoiceAccount(Convert.ToInt32(context.PartnerId));
            if (invoiceAccount.CarrierProfileId.HasValue)
            {
                return invoiceAccount.CarrierProfileId.Value;
            }
            else
            {
                return invoiceAccount.CarrierAccountId.Value;
            }
        }
        private void AddRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter key, string value, bool isVisible)
        {
            if (rdlcReportParameters == null)
                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }

        public override int? GetPartnerTimeZoneId(IPartnerTimeZoneContext context)
        {
            InvoiceAccountManager invoiceAccountManager = new Business.InvoiceAccountManager();
            var invoiceAccount = invoiceAccountManager.GetInvoiceAccount(Convert.ToInt32(context.PartnerId));
            if (invoiceAccount.CarrierProfileId.HasValue)
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                return carrierProfileManager.GetCustomerTimeZoneId(invoiceAccount.CarrierProfileId.Value);
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                return carrierAccountManager.GetCustomerTimeZoneId(invoiceAccount.CarrierAccountId.Value);
            }
        }

        public override VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context)
        {
            InvoiceAccountManager invoiceAccountManager = new Business.InvoiceAccountManager();
            var invoiceAccount = invoiceAccountManager.GetInvoiceAccount(Convert.ToInt32(context.PartnerId));
            VRAccountStatus status = VRAccountStatus.InActive;
            if (invoiceAccount.CarrierAccountId.HasValue)
            {
                if(new CarrierAccountManager().IsCarrierAccountActive(invoiceAccount.CarrierAccountId.Value))
                {
                    status = VRAccountStatus.Active;
                }
            }else if(invoiceAccount.CarrierProfileId.HasValue)
            {
                var carrierProfileAcivationStatus = new CarrierProfileManager().GetCarrierProfileActivationStatus(invoiceAccount.CarrierProfileId.Value);
               if(carrierProfileAcivationStatus == CarrierProfileActivationStatus.Active)
                     status = VRAccountStatus.Active;
            }
           
            return new VRInvoiceAccountData
            {
                BED = invoiceAccount.BED,
                EED = invoiceAccount.EED,
                Status = status
            };
        }
    }
      
}
