using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountDefinitionManager
    {
        static Vanrise.Common.Business.VRComponentTypeManager s_componentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
        public FinancialAccountDefinitionSettings GetFinancialAccountDefinitionSettings(Guid financialAccountDefinitionId)
        {
            var financialAccountDefinitionSettings = s_componentTypeManager.GetComponentTypeSettings<FinancialAccountDefinitionSettings>(financialAccountDefinitionId);
            financialAccountDefinitionSettings.ThrowIfNull("financialAccountDefinitionSettings");
            return financialAccountDefinitionSettings;
        }
    }
}
