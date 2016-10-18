using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    //public class TemplateConfigDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ITemplateConfigDataManager
    //{
    //    public TemplateConfigDataManager()
    //        : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
    //    {

    //    }

    //    public List<Entities.TemplateConfig> GetTemplateConfigurations(string configType)
    //    {
    //        return GetItemsSP("common.sp_TemplateConfig_GetByConfigType", TemplateConfigMapper, configType);
    //    }


    //    public List<Entities.TemplateConfig> GetTemplateConfigurations()
    //    {
    //        return GetItemsSP("common.sp_TemplateConfig_GetAll", TemplateConfigMapper);
    //    }

    //    public bool AreTemplateConfigurationsUpdated(ref object updateHandle)
    //    {
    //        return base.IsDataUpdated("common.TemplateConfig", ref updateHandle);
    //    }

    //    TemplateConfig TemplateConfigMapper(IDataReader reader)
    //    {
    //        TemplateConfig templateConfig = new TemplateConfig
    //        {
    //            TemplateConfigID = (int)reader["ID"],
    //            Name = reader["Name"] as string,
    //            ConfigType = reader["ConfigType"] as string,
    //            Editor = reader["Editor"] as string,
    //            BehaviorFQTN = reader["BehaviorFQTN"] as string
    //        };

    //        string settingsAsString = reader["Settings"] as string;
    //        if (settingsAsString != null)
    //            templateConfig.Settings = Serializer.Deserialize(settingsAsString);

    //        return templateConfig;
    //    }
    //}
}
