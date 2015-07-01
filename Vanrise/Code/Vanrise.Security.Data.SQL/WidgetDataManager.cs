﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class WidgetDataManager : BaseSQLDataManager, IWidgetsDataManager
    {
       public List<WidgetDefinition> GetWidgetsDefinition()
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
        public bool SaveWidget(Widget widget, out int insertedId)
        {
            string serialziedSetting = null;
            if (widget.Setting != null)
                serialziedSetting = Common.Serializer.Serialize(widget.Setting);
            object widgetId;
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Widget_Insert", out widgetId,widget.WidgetDefinitionId, widget.Name, serialziedSetting);
            insertedId = (int)widgetId;
            return (recordesEffected > 0);
            //  return false;
        }
        public bool UpdateWidget(Widget widget)
        {
            string serialziedSetting = null;
            if (widget.Setting != null)
                serialziedSetting = Common.Serializer.Serialize(widget.Setting);
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Widget_Update", widget.Id, widget.WidgetDefinitionId, widget.Name, serialziedSetting);
            return (recordesEffected > 0);
        }
        public List<WidgetDetails> GetAllWidgets()
        {

            return GetItemsSP("sec.sp_Widget_GetAll", WidgetMapper);
        }
        private WidgetDetails WidgetMapper(IDataReader reader)
        {
            WidgetDetails instance = new WidgetDetails
            {
                Id = (int)reader["Id"],
                Name = reader["WidgetName"] as string,
                WidgetDefinitionId = GetReaderValue<int>(reader, "WidgetDefinitionId"),
                WidgetDefinitionName = reader["WidgetDefinitionName"] as string,
                DirectiveName = reader["DirectiveName"] as string,
                WidgetDefinitionSetting = Vanrise.Common.Serializer.Deserialize<WidgetDefinitionSetting>(reader["WidgetDefinitionSetting"] as string),
                Setting = Vanrise.Common.Serializer.Deserialize<WidgetSetting>(reader["Setting"] as string)
            };
            return instance;
        }
        public WidgetDetails GetWidgetById(int widgetId)
        {
            return GetItemSP("sec.sp_Widget_GetById", WidgetMapper, widgetId);
        }

      
    }
}
