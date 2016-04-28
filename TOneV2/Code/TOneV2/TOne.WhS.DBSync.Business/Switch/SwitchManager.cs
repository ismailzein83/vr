using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class SwitchManager
    {

        public void AddSwitchesFromSource(List<Switch> switches)
        {
            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            dataManager.ApplySwitchesToDB(switches);
        }
        public List<TemplateConfig> GetSwitchSourceTemplates()
        {

            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SourceSwitchReaderConfigType);
        }
    }
}
