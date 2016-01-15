using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class WidgetDefinitionDataManager:BaseSQLDataManager,IWidgetDefinitionDataManager
    {
        public WidgetDefinitionDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }
        public bool AreWidgetDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[sec].[WidgetDefinition]", ref updateHandle);
        }

        public List<Entities.WidgetDefinition> GetWidgetsDefinition()
        {
            return GetItemsSP("sec.sp_WidgetDefinition_Get", WidgetDefinitionMapper);
        }
        private WidgetDefinition WidgetDefinitionMapper(IDataReader reader)
        {
            WidgetDefinition instance = new WidgetDefinition
            {
                ID = (int)reader["ID"],
                Name = reader["Name"] as string,
                DirectiveName = reader["DirectiveName"] as string,
                Setting = Vanrise.Common.Serializer.Deserialize<WidgetDefinitionSetting>(reader["Setting"] as string)
            };
            return instance;
        }
    }
}
