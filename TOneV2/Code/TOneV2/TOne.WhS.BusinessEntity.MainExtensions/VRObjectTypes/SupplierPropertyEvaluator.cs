using System;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum SupplierField { SupplierName = 0, BillingEmail = 1, PricingEmail = 2, AccountManagerEmail = 3, BillingContact = 4, PricingContact = 5, AlertingSMSPhoneNumbers = 6, SupportEmail = 8, TimeZone = 9 }

    public class SupplierPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("A1DF8B36-6784-4A44-94E0-6381F1339BA9"); }
        }

        public SupplierField SupplierField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            CarrierAccount supplier = context.Object as CarrierAccount;

            if (supplier == null)
                return null;

            switch (this.SupplierField)
            {
                case MainExtensions.SupplierField.SupplierName:
                    return new CarrierAccountManager().GetCarrierAccountName(supplier.CarrierAccountId);
                case MainExtensions.SupplierField.BillingEmail:
                    return this.GetCarrierContactDescritpion(supplier.CarrierProfileId, CarrierContactType.BillingEmail);
                case MainExtensions.SupplierField.AccountManagerEmail:
                    return this.GetCarrierContactDescritpion(supplier.CarrierProfileId, CarrierContactType.AccountManagerEmail);
                case MainExtensions.SupplierField.PricingEmail:
                    return this.GetCarrierContactDescritpion(supplier.CarrierProfileId, CarrierContactType.PricingEmail);
                case MainExtensions.SupplierField.BillingContact:
                    return this.GetCarrierContactDescritpion(supplier.CarrierProfileId, CarrierContactType.BillingContactPerson);
                case MainExtensions.SupplierField.PricingContact:
                    return this.GetCarrierContactDescritpion(supplier.CarrierProfileId, CarrierContactType.PricingContactPerson);
                case MainExtensions.SupplierField.AlertingSMSPhoneNumbers:
                    return this.GetCarrierContactDescritpion(supplier.CarrierProfileId, CarrierContactType.AlertingSMSPhoneNumbers);
                case MainExtensions.SupplierField.SupportEmail:
                    return this.GetCarrierContactDescritpion(supplier.CarrierProfileId, CarrierContactType.SupportEmail);
                case SupplierField.TimeZone:
                    var timeZoneId = new CarrierAccountManager().GetSupplierTimeZoneId(supplier.CarrierAccountId);
                    return new VRTimeZoneManager().GetVRTimeZoneName(timeZoneId);
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
