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
           
             bool AddWidget(Widget widget, out int insertedId);
             bool UpdateWidget(Widget widget);
             List<Widget> GetAllWidgets();
             int CheckWidgetSetting(Widget widget);
             bool DeleteWidget(int widgetId);
             bool AreAllWidgetsUpdated(ref object updateHandle);
        }
    
}
