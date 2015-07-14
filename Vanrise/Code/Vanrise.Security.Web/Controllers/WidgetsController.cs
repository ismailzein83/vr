using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    public class WidgetsController:Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<WidgetDefinition> GetWidgetsDefinition()
        {
            WidgetsManager manager = new WidgetsManager();
            return manager.GetWidgetsDefinition();
        }

        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<WidgetDetails> SaveWidget(Widget widget)
        {
            WidgetsManager manager = new WidgetsManager();
            return manager.SaveWidget(widget);
        }
        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<WidgetDetails> UpdateWidget(Widget widget)
        {
            WidgetsManager manager = new WidgetsManager();
            return manager.UpdateWidget(widget);
        }
         [HttpGet]
        public List<WidgetDetails> GetAllWidgets()
        {
            WidgetsManager manager = new WidgetsManager();
            return manager.GetAllWidgets();
        }


         [HttpPost]
         public List<WidgetDetails> GetFilteredWidgets(WidgetFilter filter)
         {
             WidgetsManager manager = new WidgetsManager();
             return manager.GetFilteredWidgets(filter);
         }
         [HttpGet]
         public Vanrise.Entities.DeleteOperationOutput<WidgetDetails> DeleteWidget(int widgetId)
         {
             WidgetsManager manager = new WidgetsManager();
             return manager.DeleteWidget(widgetId);
         }
    }
}