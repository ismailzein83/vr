using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.AccountManager.Business
{
    public class AccountManagerBEDefinitionFilter : IBusinessEntityDefinitionFilter
    {
        public bool IsMatched(IBusinessEntityDefinitionFilterContext context)
        {
            if (context.entityDefinition == null || context.entityDefinition.Settings == null)
                return false;
            if (context.entityDefinition.Settings.ConfigId != AccountManagerBEDefinitionSettings.s_configId)
                return false;
            return true;
        }
    }
}
