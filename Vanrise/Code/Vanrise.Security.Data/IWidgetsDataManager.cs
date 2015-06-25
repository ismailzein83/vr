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
            List<Widget> GetAllWidgets();
        }
    
}
