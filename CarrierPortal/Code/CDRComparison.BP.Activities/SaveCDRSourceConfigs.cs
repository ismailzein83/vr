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

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            int? systemConfigId = this.SystemCDRSourceConfigId.Get(context);
            int? partnerConfigId = this.PartnerCDRSourceConfigId.Get(context);

            string systemConfigName = this.SystemCDRSourceConfigName.Get(context);
            string partnerConfigName = this.PartnerCDRSourceConfigName.Get(context);

            CDRSource systemCDRSource = this.SystemCDRSource.Get(context);
            CDRSource partnerCDRSource = this.PartnerCDRSource.Get(context);
            
            SaveCDRSourceConfig(systemConfigId, systemConfigName, systemCDRSource, false);
            SaveCDRSourceConfig(partnerConfigId, partnerConfigName, partnerCDRSource, true);
        }

        #region Private Methods

        void SaveCDRSourceConfig(int? cdrSourceConfigId, string cdrSourceConfigName, CDRSource cdrSource, bool isPartnerCDRSource)
        {
            ICDRSourceConfigDataManager cdrSourceConfigDataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRSourceConfigDataManager>();

            var cdrSourceConfig = new CDRSourceConfig()
            {
                Name = cdrSourceConfigName,
                CDRSource = cdrSource,
                IsPartnerCDRSource = isPartnerCDRSource
            };

            if (cdrSourceConfigId != null)
            {
                cdrSourceConfig.CDRSourceConfigId = (int)cdrSourceConfigId;
                if (!cdrSourceConfigDataManager.UpdateCDRSourceConfig(cdrSourceConfig))
                    throw new Exception("Update failed");
            }
            else
            {
                int insertedId;
                if (!cdrSourceConfigDataManager.InsertCDRSourceConfig(cdrSourceConfig, out insertedId))
                    throw new Exception("Insert failed");
            }
        }
        
        #endregion
    }
}
