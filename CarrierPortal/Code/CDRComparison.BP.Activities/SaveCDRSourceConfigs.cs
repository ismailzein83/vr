using CDRComparison.Data;
using CDRComparison.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.BP.Activities
{
    public class SaveCDRSourceConfigs : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<bool> SaveSystemCDRSourceConfig { get; set; }
        [RequiredArgument]
        public InArgument<bool> SavePartnerCDRSourceConfig { get; set; }

        public InArgument<int?> SystemCDRSourceConfigId { get; set; }
        public InArgument<int?> PartnerCDRSourceConfigId { get; set; }

        [RequiredArgument]
        public InArgument<string> SystemCDRSourceConfigName { get; set; }
        [RequiredArgument]
        public InArgument<string> PartnerCDRSourceConfigName { get; set; }

        [RequiredArgument]
        public InArgument<CDRSource> SystemCDRSource { get; set; }
        [RequiredArgument]
        public InArgument<CDRSource> PartnerCDRSource { get; set; }

        public InArgument<SettingsTaskExecutionInfo> SettingsTaskExecutionInfo { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            bool saveSystemConfig = this.SaveSystemCDRSourceConfig.Get(context);
            bool savePartnerConfig = this.SavePartnerCDRSourceConfig.Get(context);

            int? systemConfigId = this.SystemCDRSourceConfigId.Get(context);
            int? partnerConfigId = this.PartnerCDRSourceConfigId.Get(context);

            string systemConfigName = this.SystemCDRSourceConfigName.Get(context);
            string partnerConfigName = this.PartnerCDRSourceConfigName.Get(context);

            CDRSource systemCDRSource = this.SystemCDRSource.Get(context);
            CDRSource partnerCDRSource = this.PartnerCDRSource.Get(context);

            SettingsTaskExecutionInfo settingsTaskExecutionInfo = this.SettingsTaskExecutionInfo.Get(context);

            if (saveSystemConfig)
                SaveCDRSourceConfig(systemConfigId, systemConfigName, systemCDRSource, settingsTaskExecutionInfo, false);
            
            if (savePartnerConfig)
                SaveCDRSourceConfig(partnerConfigId, partnerConfigName, partnerCDRSource, settingsTaskExecutionInfo, true);
        }

        #region Private Methods

        void SaveCDRSourceConfig(int? cdrSourceConfigId, string cdrSourceConfigName, CDRSource cdrSource, SettingsTaskExecutionInfo settingsTaskExecutionInfo, bool isPartnerCDRSource)
        {
            ICDRSourceConfigDataManager cdrSourceConfigDataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRSourceConfigDataManager>();

            var cdrSourceConfig = new CDRSourceConfig()
            {
                Name = cdrSourceConfigName,
                CDRSource = cdrSource,
                IsPartnerCDRSource = isPartnerCDRSource
            };

            if (isPartnerCDRSource)
            {
                cdrSourceConfig.SettingsTaskExecutionInfo = settingsTaskExecutionInfo;
            }

            if (cdrSourceConfigId != null)
            {
                cdrSourceConfig.CDRSourceConfigId = (int)cdrSourceConfigId;
                if (!cdrSourceConfigDataManager.UpdateCDRSourceConfig(cdrSourceConfig))
                    throw new Exception("Update failed: Another CDR source configuration with the same name already exists");
            }
            else
            {
                int insertedId;
                if (!cdrSourceConfigDataManager.InsertCDRSourceConfig(cdrSourceConfig, out insertedId))
                    throw new Exception("Insert failed: Another CDR source configuration with the same name already exists");
            }
        }
        
        #endregion
    }
}
