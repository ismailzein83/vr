using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartCompanyProfile : AccountPartSettings, IAccountProfile
    {
        private static CityManager s_cityManager = new CityManager();

        public override Guid ConfigId { get { return _ConfigId; } }
        public static Guid _ConfigId = new Guid("B0717C4F-E409-4AE2-8C00-5ADD4CA828C5");

        public const int ExtensionConfigId = 21;
        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public string Town { get; set; }

        public string Street { get; set; }

        public string POBox { get; set; }

        public string Website { get; set; }

        public string ArabicName { get; set; }

        public Dictionary<string, AccountCompanyContact> Contacts { get; set; }

        #region IAccountProfile Memebers

        public List<string> PhoneNumbers { get; set; }
        public List<string> MobileNumbers { get; set; }
        public List<string> Faxes { get; set; }

        public string Address { get; set; }

        #endregion


        public override dynamic GetFieldValue(IAccountPartGetFieldValueContext context)
        {
            switch (context.FieldName)
            {
                case "ArabicName": return this.ArabicName;
                case "Country": return this.CountryId;
                case "Region": return GetRegionId();
                default:
                    return GetContactFieldValue(context.FieldName);

            }


        }
        public bool TryGetContact(string contactType, out AccountContact accountContact)
        {
            if (this.Contacts != null && this.Contacts.Count != 0)
            {
                var mainContact = this.Contacts.GetRecord(contactType);
                if (mainContact != null)
                {
                    accountContact = new AccountContact
                    {
                        ContactName = mainContact.ContactName,
                        PhoneNumbers = mainContact.PhoneNumbers
                    };
                    return true;
                }
                accountContact = null;
                return false;
            }
            accountContact = null;
            return false;
        }


        private int? GetRegionId()
        {
            if (this.CityId.HasValue)
                return s_cityManager.GetCityRegionId(this.CityId.Value);
            else
                return null;
        }

        dynamic GetContactFieldValue(string FieldName)
        {
            if (FieldName.Contains("Name"))
            {
                List<string> namePart = FieldName.Split('_').ToList();
                var contact = this.Contacts.GetRecord(namePart[0]);
                return contact != null ? contact.ContactName : null;
            }
            if (FieldName.Contains("Email"))
            {
                List<string> namePart = FieldName.Split('_').ToList();
                var contact = this.Contacts.GetRecord(namePart[0]);
                return contact != null ? contact.Email : null;
            }
            if (FieldName.Contains("PhoneNumbers"))
            {
                List<string> namePart = FieldName.Split('_').ToList();
                var contact = this.Contacts.GetRecord(namePart[0]);
                return contact != null && contact.PhoneNumbers != null ? string.Join(",", contact.PhoneNumbers) : null;
            }
            if (FieldName.Contains("MobileNumbers"))
            {
                List<string> namePart = FieldName.Split('_').ToList();
                var contact = this.Contacts.GetRecord(namePart[0]);
                return contact != null && contact.MobileNumbers != null ? string.Join(",", contact.MobileNumbers) : null;
            }
            if (FieldName.Contains("Title"))
            {
                List<string> namePart = FieldName.Split('_').ToList();
                var contact = this.Contacts.GetRecord(namePart[0]);
                return contact != null ? contact.Title : null;
            }
            if (FieldName.Contains("Notes"))
            {
                List<string> namePart = FieldName.Split('_').ToList();
                var contact = this.Contacts.GetRecord(namePart[0]);
                return contact != null ? contact.Notes : null;
            }
            else
                return null;
        }
    }

    public class AccountCompanyContact
    {
        public string ContactName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public List<string> MobileNumbers { get; set; }
        public SalutationType? Salutation { get; set; }
        public string Notes { get; set; }
    }
}
