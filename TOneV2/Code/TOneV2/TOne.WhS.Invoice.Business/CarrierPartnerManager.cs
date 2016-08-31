using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business
{
    public class CarrierPartnerManager : IPartnerManager
    {

        public dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            switch(context.InfoType)
            {
                case "InvoiceRDLCReport":
                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                     string[] partnerId = context.PartnerId.Split('_');
                     if (partnerId[0].Equals("Profile"))
                     {
                         CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                         var carrierProfile = carrierProfileManager.GetCarrierProfile(Convert.ToInt32(partnerId[1]));
                         if (carrierProfile != null)
                         {
                             rdlcReportParameters.Add("Customer", new VRRdlcReportParameter { Value = carrierProfileManager.GetCarrierProfileName(carrierProfile.CarrierProfileId), IsVisible = true });
                             if(carrierProfile.Settings.Address != null)
                                 rdlcReportParameters.Add("Address", new VRRdlcReportParameter { Value = carrierProfile.Settings.Address, IsVisible = true });
                             if (carrierProfile.Settings.PhoneNumbers != null)
                                 rdlcReportParameters.Add("Phone", new VRRdlcReportParameter { Value = String.Join(" ",carrierProfile.Settings.PhoneNumbers) ,IsVisible = true});
                             if (carrierProfile.Settings.Faxes != null)
                                 rdlcReportParameters.Add("Fax", new VRRdlcReportParameter { Value = String.Join(" ", carrierProfile.Settings.Faxes), IsVisible = true });
                             if (carrierProfile.Settings.CompanyLogo != null)
                             {
                                 VRFileManager fileManager = new VRFileManager();
                                 var logo = fileManager.GetFile(carrierProfile.Settings.CompanyLogo);
                                 rdlcReportParameters.Add("Image", new VRRdlcReportParameter { Value = Convert.ToBase64String(logo.Content), IsVisible = true });
                             }
                         }
                     }
                     else if (partnerId[0].Equals("Account"))
                     {
                         CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                         var carrierAccount = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(partnerId[1]));
                         CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                         var carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);
                         if (carrierProfile != null)
                         {
                             rdlcReportParameters.Add("Customer", new VRRdlcReportParameter { Value = carrierAccountManager.GetCarrierAccountName(carrierAccount.CarrierAccountId), IsVisible = true });
                             if (carrierProfile.Settings.Address != null)
                                 rdlcReportParameters.Add("Address", new VRRdlcReportParameter { Value = carrierProfile.Settings.Address, IsVisible = true });
                             if (carrierProfile.Settings.PhoneNumbers != null)
                                 rdlcReportParameters.Add("Phone", new VRRdlcReportParameter { Value = String.Join(" ", carrierProfile.Settings.PhoneNumbers), IsVisible = true });
                             if (carrierProfile.Settings.Faxes != null)
                                 rdlcReportParameters.Add("Fax", new VRRdlcReportParameter { Value = String.Join(" ", carrierProfile.Settings.Faxes), IsVisible = true });
                             if (carrierProfile.Settings.CompanyLogo != null)
                             {
                                 VRFileManager fileManager = new VRFileManager();
                                 var logo = fileManager.GetFile(carrierProfile.Settings.CompanyLogo);
                                 rdlcReportParameters.Add("Image", new VRRdlcReportParameter { Value =Convert.ToBase64String(logo.Content), IsVisible = true });
                             }
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
    }
}
