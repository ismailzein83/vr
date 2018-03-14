using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum CarrierProfileActivationStatus { Active = 0, InActive = 1 }
    public class BaseCarrierProfile
    {
        public int CarrierProfileId { get; set; }

        public string Name { get; set; }

        public CarrierProfileSettings Settings { get; set; }

        public string SourceId { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }

    public class CarrierProfileSettings
    {
        public CarrierProfileActivationStatus CustomerActivationStatus { get; set; }
        public CarrierProfileActivationStatus SupplierActivationStatus { get; set; }

        public string Company { get; set; }

        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public string RegistrationNumber { get; set; }

        public List<string> PhoneNumbers { get; set; }

        public List<string> Faxes { get; set; }

        public string Website { get; set; }

        public string Address { get; set; }

        public string PostalCode { get; set; }

        public string Town { get; set; }

        public long? CompanyLogo { get; set; }
        public int DefaultCusotmerTimeZoneId { get; set; }
        public int DefaultSupplierTimeZoneId { get; set; }
        public int CurrencyId { get; set; }
        public Guid? InvoiceSettingId { get; set; }
        public Guid? CompanySettingId { get; set; }
        public List<CarrierContact> Contacts { get; set; }

        public List<CarrierProfileTicketContact> TicketContacts { get; set; }

        public VRTaxSetting TaxSetting { get; set; }
        public List<VRDocumentSetting> Documents { get; set; }
    }

    public class CarrierProfile: BaseCarrierProfile
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "WHS_BE_CarrierProfile";
        public Dictionary<string, Object> ExtendedSettings { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class CarrierProfileTicketContact
    {
        public Guid CarrierProfileTicketContactId { get; set; }

        public string Name { get; set; }

        public List<string> PhoneNumber { get; set; }

        public List<string> Emails { get; set; }
    }

    public class CarrierProfileTicketContactInfo
    {
        public Guid CarrierProfileTicketContactId { get; set; }

        public string Name { get; set; }
        public string NameDescription { get; set; }
        public List<string> PhoneNumber { get; set; }

        public List<string> Emails { get; set; }
    }

    public class TicketContactInfoFilter
    {
        public int CarrierAccountId { get; set; }
    }
    public class CarrierProfileToEdit : BaseCarrierProfile
    {

    }
}
