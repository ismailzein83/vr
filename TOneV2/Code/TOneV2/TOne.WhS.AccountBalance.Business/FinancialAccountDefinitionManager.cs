using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;
using Vanrise.Common;

namespace TOne.WhS.AccountBalance.Business
{
    public class FinancialAccountDefinitionManager
    {
        static Vanrise.Common.Business.VRComponentTypeManager s_componentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
        public T GetFinancialAccountDefinitionExtendedSettings<T>(Guid financialAccountDefinitionId) where T : FinancialAccountDefinitionExtendedSettings
        {
            FinancialAccountDefinitionSettings definitionSettings = GetFinancialAccountDefinitionSettings(financialAccountDefinitionId);
            return definitionSettings.ExtendedSettings.CastWithValidate<T>("definitionSettings.ExtendedSettings", financialAccountDefinitionId);
        }

        public FinancialAccountDefinitionSettings GetFinancialAccountDefinitionSettings(Guid financialAccountDefinitionId)
        {
            FinancialAccountDefinitionSettings definitionSettings = s_componentTypeManager.GetComponentTypeSettings<FinancialAccountDefinitionSettings>(financialAccountDefinitionId);
            definitionSettings.ThrowIfNull("definitionSettings", financialAccountDefinitionId);
            return definitionSettings;
        }
    }
}
