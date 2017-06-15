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
    public class AccountPartResidentialProfileDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("747d0c68-a508-4aa3-8d02-0d3cdfe72149"); } }

        static CountryManager s_countryManager = new CountryManager();
        static CityManager s_cityManager = new CityManager();

        public override List<GenericFieldDefinition> GetFieldDefinitions()
        {
            return new List<GenericFieldDefinition>()
                {
                    new GenericFieldDefinition()
                    {
                        Name = "Email",
                        Title = "Email",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    }
                };
        }

        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartResidentialProfile part = context.AccountPartSettings.CastWithValidate<AccountPartResidentialProfile>("context.AccountPartSettings");
            if (part.CountryId.HasValue && s_countryManager.GetCountry(part.CountryId.Value) == null)
                context.ErrorMessage = String.Format("Country '{0}' not found", part.CountryId.Value);

            if (part.CityId.HasValue && s_cityManager.GetCity(part.CityId.Value) == null)
                context.ErrorMessage = String.Format("City '{0}' not found", part.CityId.Value);
            return true;
        }

    }
}
