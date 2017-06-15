using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartCompanyProfileDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("1aff2bf7-1f15-4e0b-accf-457edf36a342"); } }

        public bool IncludeArabicName { get; set; }

        public List<CompanyProfileContactType> ContactTypes { get; set; }

        static CountryManager s_countryManager = new CountryManager();
        static CityManager s_cityManager = new CityManager();
        public override List<GenericFieldDefinition> GetFieldDefinitions()
        {
            var list = new List<GenericFieldDefinition>();

            if(this.ContactTypes != null)
            foreach (var a in this.ContactTypes)
            {
                list.Add(new GenericFieldDefinition()
                {
                    Name = string.Format("{0}_Name",a.Name),
                    Title = string.Format("{0} Name", a.Name),
                    FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                });
                list.Add(new GenericFieldDefinition()
                {
                    Name = string.Format("{0}_Email", a.Name),
                    Title = string.Format("{0} Email", a.Name),
                    FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                });
                list.Add(new GenericFieldDefinition()
                {
                    Name = string.Format("{0}_PhoneNumbers", a.Name),
                    Title = string.Format("{0} Phone Numbers", a.Name),
                    FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                });
                list.Add(new GenericFieldDefinition()
                {
                    Name = string.Format("{0}_Title", a.Name),
                    Title = string.Format("{0} Title", a.Name),
                    FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                });
            }

            list.Add(new GenericFieldDefinition
            {
                Name = "Country",
                Title = "Country",
                FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType { BusinessEntityDefinitionId = Vanrise.Entities.Country.BUSINESSENTITY_DEFINITION_ID }
            });

            list.Add(new GenericFieldDefinition
            {
                Name = "Region",
                Title = "Region",
                FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType { BusinessEntityDefinitionId = Vanrise.Entities.Region.BUSINESSENTITY_DEFINITION_ID }
            });

           if (this.IncludeArabicName == true)
               list.Add(new GenericFieldDefinition()
                    {
                        Name = "ArabicName",
                        Title = "Arabic Name",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    });
           return list;
        }

        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartCompanyProfile part = context.AccountPartSettings.CastWithValidate<AccountPartCompanyProfile>("context.AccountPartSettings");
            if (part.CountryId.HasValue && s_countryManager.GetCountry(part.CountryId.Value) == null)
                context.ErrorMessage = String.Format("Country '{0}' not found", part.CountryId.Value);

            if (part.CityId.HasValue && s_cityManager.GetCity(part.CityId.Value) == null)
                context.ErrorMessage = String.Format("City '{0}' not found", part.CityId.Value);

            return true;
        }
    }
    public class CompanyProfileContactType
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
