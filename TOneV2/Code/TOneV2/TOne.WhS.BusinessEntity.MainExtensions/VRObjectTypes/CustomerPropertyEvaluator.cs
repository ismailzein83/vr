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
    public enum CustomerField { CarrierName = 0, BillingEmail = 1, PricingEmail = 2, AccountManagerEmail = 3 }

    public class CustomerPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("2647ede8-0ba5-4131-8f2f-eb047ff0b359"); } 
        }

        public CustomerField CustomerField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            CarrierAccount customer = context.Object as CarrierAccount;

            if (customer == null)
                throw new NullReferenceException("customer");

            switch (this.CustomerField)
            {
                case MainExtensions.CustomerField.CarrierName:
                    return new CarrierAccountManager().GetCarrierAccountName(customer.CarrierAccountId);
                case MainExtensions.CustomerField.BillingEmail:
                    return this.GetCarrierContactDescritpion(customer.CarrierProfileId, CarrierContactType.BillingEmail);
                case MainExtensions.CustomerField.AccountManagerEmail:
                    return this.GetCarrierContactDescritpion(customer.CarrierProfileId, CarrierContactType.AccountManagerEmail);
                case MainExtensions.CustomerField.PricingEmail:
                    return this.GetCarrierContactDescritpion(customer.CarrierProfileId, CarrierContactType.PricingEmail);
            }

            return null;
        }

        private string GetCarrierContactDescritpion(int carrierProfileId, CarrierContactType contactType)
        {
            CarrierProfile carrierProfile = new CarrierProfileManager().GetCarrierProfile(carrierProfileId);

            if (carrierProfile.Settings == null)
                throw new NullReferenceException("carrierprofile settings");
            if (carrierProfile.Settings.Contacts == null)
                throw new NullReferenceException("carrierprofile contacts");

            CarrierContact contact = carrierProfile.Settings.Contacts.FindRecord(x => x.Type == contactType);
            if (contact != null)
                return contact.Description;

            return null;
        }
    }
}
