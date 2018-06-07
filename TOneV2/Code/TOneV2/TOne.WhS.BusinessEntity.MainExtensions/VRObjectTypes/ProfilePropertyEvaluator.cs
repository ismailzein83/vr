using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Common;
namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum CarrierProfileField { CustomerName = 0, BillingEmail = 1, PricingEmail = 2, AccountManagerEmail = 3, AlertingSMSPhoneNumbers = 4, ProfileCompanyName = 5 }

    public class ProfilePropertyEvaluator: VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("7A25EC81-E397-4932-A54F-832DDDE4C734"); } 
        }

        public CarrierProfileField ProfileField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            CarrierProfile profile = context.Object as CarrierProfile;

            if (profile == null)
                throw new NullReferenceException("profile");

            switch (this.ProfileField)
            {
                case MainExtensions.CarrierProfileField.CustomerName:
                    return profile.Name;
                case MainExtensions.CarrierProfileField.BillingEmail:
                    return this.GetCarrierContactDescritpion(profile, CarrierContactType.BillingEmail);
                case MainExtensions.CarrierProfileField.AccountManagerEmail:
                    return this.GetCarrierContactDescritpion(profile, CarrierContactType.AccountManagerEmail);
                case MainExtensions.CarrierProfileField.PricingEmail:
                    return this.GetCarrierContactDescritpion(profile, CarrierContactType.PricingEmail);
                case MainExtensions.CarrierProfileField.AlertingSMSPhoneNumbers:
                    return this.GetCarrierContactDescritpion(profile, CarrierContactType.AlertingSMSPhoneNumbers);
                case MainExtensions.CarrierProfileField.ProfileCompanyName:
                    return profile.Settings.Company;
            }

            return null;
        }

        private string GetCarrierContactDescritpion(CarrierProfile profile, CarrierContactType contactType)
        {
            if (profile.Settings == null)
                throw new NullReferenceException("carrierprofile settings");
            if (profile.Settings.Contacts == null)
                throw new NullReferenceException("carrierprofile contacts");

            CarrierContact contact = profile.Settings.Contacts.FindRecord(x => x.Type == contactType);
            if (contact != null)
                return contact.Description;

            return null;
        }
    }
}
