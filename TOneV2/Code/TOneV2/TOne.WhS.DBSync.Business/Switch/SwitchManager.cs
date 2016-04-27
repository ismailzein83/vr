using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;


namespace TOne.WhS.DBSync.Business
{
    public class SwitchManager
    {

        public void AddSwitchFromSource(Switch whsSwitch)
        {
            ISwitchDataManager dataManager = TOne.WhS.DBSync.Data.BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            dataManager.InsertSwitchFromSource(whsSwitch);
        }
        public List<Vanrise.Entities.TemplateConfig> GetSwitchSourceTemplates()
        {

            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SourceSwitchReaderConfigType);
        }
    }
}
