using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartPersonalInfoDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("3900317c-b982-4d8b-bd0d-01215ac1f3d9"); } }

        static CountryManager s_countryManager = new CountryManager();
        static CityManager s_cityManager = new CityManager();

        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartPersonalInfo part = context.AccountPartSettings.CastWithValidate<AccountPartPersonalInfo>("context.AccountPartSettings");
            if (part.CountryId.HasValue && s_countryManager.GetCountry(part.CountryId.Value) == null)
                context.ErrorMessage = String.Format("Country '{0}' not found", part.CountryId.Value);

            if (part.CityId.HasValue && s_cityManager.GetCity(part.CityId.Value) == null)
                context.ErrorMessage = String.Format("City '{0}' not found", part.CityId.Value);
            return true;
        }
    }
}
