using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities.RecurringCharges;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business.RecurringCharges
{
    public class RecurringChargeManager 
    {
        public IEnumerable<RecurringChargePeriodConfig> GetRecurringChargePeriodsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<RecurringChargePeriodConfig>(RecurringChargePeriodConfig.EXTENSION_TYPE);
        }
    }
}
