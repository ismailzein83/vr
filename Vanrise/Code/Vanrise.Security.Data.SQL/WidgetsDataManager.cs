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
    public class WidgetsDataManager : BaseSQLDataManager, IWidgetsDataManager
    {
       public List<WidgetDefinition> GetWidgetsDefinition()
        {

            return GetItemsSP("sec.sp_WidgetDefinition_GetWidgets", WidgetDefinitionMapper);
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
        public bool SaveWidget(Widget widget, out int insertedId)
        {
            string serialziedSetting = null;
            if (widget.Setting != null)
                serialziedSetting = Common.Serializer.Serialize(widget.Setting);
            object widgetId;
            int recordesEffected = ExecuteNonQuerySP("sec.sp_WidgetManagement_InsertWidget", out widgetId,widget.WidgetDefinitionId, widget.Name, serialziedSetting);
            insertedId = (int)widgetId;
            return (recordesEffected > 0);
            //  return false;
        }
        public List<Widget> GetAllWidgets()
        {

            return GetItemsSP("sec.sp_WidgetManagement_GetAllWidgets", WidgetMapper);
        }
        private Widget WidgetMapper(IDataReader reader)
        {  
            Widget instance = new Widget
            {
                Id = (int)reader["Id"],
                Name = reader["WidgetName"] as string,
                WidgetDefinitionId = GetReaderValue<int>(reader, "WidgetDefinitionId"),
                WidgetDefinition = new WidgetDefinition
                {
                    ID = GetReaderValue<int>(reader, "WidgetDefinitionId"),
                    Name = reader["WidgetDefinitionName"] as string,
                    DirectiveName = reader["DirectiveName"] as string,
                },
                Setting = Vanrise.Common.Serializer.Deserialize<WidgetSetting>(reader["Setting"] as string)
            };
            return instance;
        }

      
    }
}
