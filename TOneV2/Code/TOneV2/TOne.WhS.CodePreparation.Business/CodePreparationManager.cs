using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;
using System.ComponentModel;

namespace TOne.WhS.CodePreparation.Business
{
    public partial class CodePreparationManager
    {
        public Changes GetChanges(int sellingNumberPlanId)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            Changes changes = dataManager.GetChanges(sellingNumberPlanId, CodePreparationStatus.Draft);
            if (changes == null)
                changes = new Changes();
            return changes;
        }

        public bool CheckCodePreparationState(int sellingNumberPlanId)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            return dataManager.CheckCodePreparationState(sellingNumberPlanId);
        }

        public bool CancelCodePreparationState(int sellingNumberPlanId)
        {
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            return dataManager.UpdateCodePreparationStatus(sellingNumberPlanId, CodePreparationStatus.Canceled);
        }

        public int GetCPEffectiveDateDayOffset()
        {
			var configManager = new TOne.WhS.BusinessEntity.Business.ConfigManager();
			return configManager.GetSaleAreaEffectiveDateDayOffset();
        }
    }
}
