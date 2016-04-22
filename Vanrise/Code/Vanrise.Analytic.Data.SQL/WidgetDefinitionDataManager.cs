using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Analytic.Data.SQL
{
    public class WidgetDefinitionDataManager:BaseSQLDataManager,IWidgetDefinitionDataManager
    {
        public WidgetDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }


        #region Public Methods
        public bool AreWidgetDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[Analytic].[WidgetDefinition]", ref updateHandle);
        }
        public List<WidgetDefinition> GetWidgetDefinitions()
        {
            return GetItemsSP("[Analytic].[sp_WidgetDefinition_GetAll]", WidgetDefinitionReader);
        }

        #endregion


        #region Mappers
        private WidgetDefinition WidgetDefinitionReader(IDataReader reader) 
        {
            return new WidgetDefinition
            {
                WidgetDefinitionId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<WidgetDefinitionSetting>(reader["Settings"] as string),
            };
        }
        #endregion
    }
}
