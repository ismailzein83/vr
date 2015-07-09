using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
   
        public interface IWidgetsDataManager : IDataManager
        {
            List<WidgetDefinition> GetWidgetsDefinition();
            bool SaveWidget(Widget widget, out int insertedId);
            bool UpdateWidget(Widget widget);
            List<WidgetDetails> GetAllWidgets();
            WidgetDetails GetWidgetById(int widgetId);

            List<WidgetDetails> GetFilteredWidgets(string filter);
             int CheckWidgetSetting(WidgetSetting setting);
        }
    
}
