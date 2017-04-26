using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ChargeableEntityDefinitionFilter : IBusinessEntityDefinitionFilter
    {
        public bool IsMatched(IBusinessEntityDefinitionFilterContext context)
        {
            if (context.entityDefinition == null || context.entityDefinition.Settings == null)
                return false;
            var settings = context.entityDefinition.Settings as GenericLKUPBEDefinitionSettings;
            if (settings == null || settings.ExtendedSettings == null)
                return false;
            if (settings.ExtendedSettings.ConfigId != ChargeableEntityDefinitionSettings.s_configId)
                return false;

            return true;
        }
    }
}
