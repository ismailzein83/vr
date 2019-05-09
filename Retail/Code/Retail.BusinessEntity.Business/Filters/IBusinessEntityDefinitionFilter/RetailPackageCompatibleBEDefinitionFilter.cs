using Retail.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class RetailPackageCompatibleBEDefinitionFilter : IBusinessEntityDefinitionFilter
    {
        public bool IsMatched(IBusinessEntityDefinitionFilterContext context)
        {
            BusinessEntityDefinition beDefinition = context.entityDefinition;
            beDefinition.ThrowIfNull("beDefinition", beDefinition.BusinessEntityDefinitionId);
            beDefinition.Settings.ThrowIfNull("beDefinition.Settings", beDefinition.BusinessEntityDefinitionId);

            var additionalSettings = beDefinition.Settings.GetAdditionalSettings(new BEDefinitionSettingsGetAdditionalSettingsContext());
            if (additionalSettings == null)
                return false;

            var accountPackageProvider = additionalSettings.GetRecord("AccountPackageProvider");
            if (accountPackageProvider == null)
                return false;

            return accountPackageProvider is AccountPackageProvider;
        }
    }
}