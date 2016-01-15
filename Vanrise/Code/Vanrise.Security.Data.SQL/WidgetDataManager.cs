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
    public class WidgetDataManager : BaseSQLDataManager, IWidgetsDataManager
    {
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static WidgetDataManager()
        {
            _columnMapper.Add("Name", "WidgetName");
        }

        public WidgetDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

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
        public bool AddWidget(Widget widget, out int insertedId)
        {
            string serialziedSetting = null;
            if (widget.Setting != null)
                serialziedSetting = Common.Serializer.Serialize(widget.Setting);
            object widgetId;
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Widget_Insert", out widgetId,widget.WidgetDefinitionId, widget.Name,widget.Title, serialziedSetting);
            insertedId = (recordesEffected > 0) ? (int)widgetId : -1;

            return (recordesEffected > 0);
            //  return false;
        }
        public bool UpdateWidget(Widget widget)
        {
            string serialziedSetting = null;
            if (widget.Setting != null)
                serialziedSetting = Common.Serializer.Serialize(widget.Setting);
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Widget_Update", widget.Id, widget.WidgetDefinitionId, widget.Name,widget.Title, serialziedSetting);
            return (recordesEffected > 0);
        }
        public bool DeleteWidget(int widgetId)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Widget_Delete", widgetId);
            return (recordesEffected > 0);
        }
        public List<Widget> GetAllWidgets()
        {

            return GetItemsSP("sec.sp_Widget_GetAll", WidgetMapper);
        }
        private Widget WidgetMapper(IDataReader reader)
        {
            Widget instance = new Widget
            {
                Id = (int)reader["Id"],
                Name = reader["WidgetName"] as string,
                Title = reader["Title"] as string,
                WidgetDefinitionId = GetReaderValue<int>(reader, "WidgetDefinitionId"),
                Setting = Vanrise.Common.Serializer.Deserialize<WidgetSetting>(reader["Setting"] as string)
            };
            return instance;
        }

        public int CheckWidgetSetting(Widget widget)
        {
             string serialziedSetting = null;
             if (widget.Setting != null)
                 serialziedSetting = Common.Serializer.Serialize(widget.Setting);

             return (int)ExecuteScalarSP("sec.sp_Widget_CheckSetting", serialziedSetting, ToDBNullIfDefault(widget.Id));
        }



        public bool AreAllWidgetsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.Widget", ref updateHandle);
        }
    }
}
