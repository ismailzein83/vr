using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum HeaderFiledType
    {
        CompanyName = 0,
        CustomerName = 1,
        PricelistDate = 2,
        PricelistCurrency = 3,
        PricelistType = 4,
        CustomerFaxes = 5,
        CustomerPhoneNumbers = 6,
        CustomerContactPerson = 7,
        CustomerAddress = 8,
    }

    public class HeaderMappedCell : MappedCell
    {
        public override Guid ConfigId
        {
            get { return new Guid("89BDA522-A5FC-4B1C-8E9B-4D390EB8F6AF"); }
        }
        public HeaderFiledType HeaderField { get; set; }

        public override void Execute(Entities.IMappedCellContext context)
        {

            switch (HeaderField)
            {
                case HeaderFiledType.CompanyName:
                    context.Value = getCompanyName(context.CustomerId);
                    break;
                case HeaderFiledType.CustomerName:
                    context.Value = getCustomerName(context.CustomerId);
                    break;
                case HeaderFiledType.PricelistDate:
                    context.Value = context.PricelistDate;
                    break;
                case HeaderFiledType.PricelistCurrency:
                    context.Value = getPricelistCurrencyName(context.PricelistCurrencyId);
                    break;
                case HeaderFiledType.PricelistType:
                    context.Value = getPricelistType(context.PricelistType);
                    break;
                case HeaderFiledType.CustomerFaxes:
                    context.Value = getCustomerFaxes(context.CustomerId);
                    break;
                case HeaderFiledType.CustomerPhoneNumbers:
                    context.Value = getCustomerPhoneNumbers(context.CustomerId);
                    break;
                case HeaderFiledType.CustomerContactPerson:
                    context.Value = getCustomerPricingContactPerson(context.CustomerId);
                    break;
                case HeaderFiledType.CustomerAddress:
                    context.Value = getCustomerAddress(context.CustomerId);
                    break;
                default:
                    context.Value = null;
                    break;
            }
        }

        private string getCompanyName(int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            return carrierAccountManager.GetCompanySetting(customerId).CompanyName;
        }
        private string getCustomerName(int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            return carrierAccountManager.GetCarrierAccountName(customerId);
        }
        private string getPricelistCurrencyName(int currencyId)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            return currencyManager.GetCurrencyName(currencyId);
        }
        private string getPricelistType(SalePriceListType pricelistType)
        {
            switch (pricelistType)
            {
                case SalePriceListType.Full:
                    return "Full";
                case SalePriceListType.Country:
                    return "Country";
                case SalePriceListType.RateChange:
                    return "Rate Change";
            }
            return null;
        }

        private string getCustomerFaxes(int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();

            var carrierProfileId = carrierAccountManager.GetCarrierProfileId(customerId);
            if (!carrierProfileId.HasValue)
                throw new DataIntegrityValidationException(string.Format("Carrier account with Id {0} does not have carrier profile", customerId));
            var carrierProfile = carrierProfileManager.GetCarrierProfile(carrierProfileId.Value);
            
            if(carrierProfile.Settings != null && carrierProfile.Settings.Faxes != null)
                return String.Join(" , ", carrierProfile.Settings.Faxes);

            return string.Empty;
        }
        private string getCustomerPhoneNumbers(int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            var carrierProfileId = carrierAccountManager.GetCarrierProfileId(customerId);
            if (!carrierProfileId.HasValue)
                throw new DataIntegrityValidationException(string.Format("Carrier Account with Id {0} does not have Carrier Profile", customerId));
            var carrierProfile = carrierProfileManager.GetCarrierProfile(carrierProfileId.Value);

            if (carrierProfile.Settings != null && carrierProfile.Settings.Faxes != null)
                return String.Join(" , ", carrierProfile.Settings.PhoneNumbers);

            return string.Empty;
        }
        private string getCustomerPricingContactPerson(int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            var carrierProfileId = carrierAccountManager.GetCarrierProfileId(customerId);
            if (!carrierProfileId.HasValue)
                throw new DataIntegrityValidationException(string.Format("Carrier Account with Id {0} does not have Carrier Profile", customerId));
            var carrierProfile = carrierProfileManager.GetCarrierProfile(carrierProfileId.Value);
            CarrierContact pricingContact = carrierProfile.Settings.Contacts.FirstOrDefault(x => x.Type == CarrierContactType.PricingContactPerson);

            return pricingContact != null ? pricingContact.Description : string.Empty;
        }
        private string getCustomerAddress(int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            var carrierProfileId = carrierAccountManager.GetCarrierProfileId(customerId);
            if (!carrierProfileId.HasValue)
                throw new DataIntegrityValidationException(string.Format("Carrier Account with Id {0} does not have Carrier Profile", customerId));
            var carrierProfile = carrierProfileManager.GetCarrierProfile(carrierProfileId.Value);

            if (carrierProfile != null && carrierProfile.Settings != null)
                return carrierProfile.Settings.Address;

            return string.Empty;
        }
    }
}