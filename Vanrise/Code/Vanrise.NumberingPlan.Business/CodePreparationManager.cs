using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.NumberingPlan.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
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

		public void CleanTemporaryTables(long processInstanceId)
		{
			ICodePreparationDataManager cpDataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
			cpDataManager.CleanTemporaryTables(processInstanceId);
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

        public NPSettingsData GetCPSettings()
        {
            SettingManager settingManager = new SettingManager();
            return settingManager.GetSetting<NPSettingsData>(Constants.CPSettings);
        }

    }
}
