using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
namespace Vanrise.GenericData.Business
{
    public class GenericBusinessEntityDefinitionFilter : IBusinessEntityDefinitionFilter
    {
        public bool IsMatched(IBusinessEntityDefinitionFilterContext context)
        {
            if (context.entityDefinition == null || context.entityDefinition.Settings == null)
                return false;

            if (context.entityDefinition.Settings.ConfigId != GenericBEDefinitionSettings.s_configId)
                return false;

            return true;
        }
    }
}
