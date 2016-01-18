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
    [RoutePrefix(Constants.ROUTE_PREFIX + "Widget")]
    public class WidgetController:Vanrise.Web.Base.BaseAPIController
    {

        [HttpPost]
        [Route("AddWidget")]
        public Vanrise.Entities.InsertOperationOutput<WidgetDetail> AddWidget(Widget widget)
        {
            WidgetsManager manager = new WidgetsManager();
            return manager.AddWidget(widget);
        }
        [HttpPost]
        [Route("UpdateWidget")]
        public Vanrise.Entities.UpdateOperationOutput<WidgetDetail> UpdateWidget(Widget widget)
        {
            WidgetsManager manager = new WidgetsManager();
            return manager.UpdateWidget(widget);
        }
         [HttpGet]
         [Route("GetAllWidgets")]
        public IEnumerable<WidgetDetail> GetAllWidgets()
        {
            WidgetsManager manager = new WidgetsManager();
            return manager.GetAllWidgets();
        }

         [HttpPost]
         [Route("GetFilteredWidgets")]
         public object GetFilteredWidgets(Vanrise.Entities.DataRetrievalInput<WidgetFilter> filter)
         {
             WidgetsManager manager = new WidgetsManager();
             return GetWebResponse(filter, manager.GetFilteredWidgets(filter));
         }
         [HttpGet]
         [Route("DeleteWidget")]
         public Vanrise.Entities.DeleteOperationOutput<WidgetDetail> DeleteWidget(int widgetId)
         {
             WidgetsManager manager = new WidgetsManager();
             return manager.DeleteWidget(widgetId);
         }
         [HttpGet]
         [Route("GetWidgetById")]
         public Widget GetWidgetById(int widgetId)
         {
             WidgetsManager manager = new WidgetsManager();
             return manager.GetWidgetById(widgetId);
         }

    }
}