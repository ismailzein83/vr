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
        WidgetsManager _manager;
        public WidgetController()
        {
            _manager = new WidgetsManager();
        }

        [HttpPost]
        [Route("AddWidget")]
        public Vanrise.Entities.InsertOperationOutput<WidgetDetail> AddWidget(Widget widget)
        {
            return _manager.AddWidget(widget);
        }

        [HttpPost]
        [Route("UpdateWidget")]
        public Vanrise.Entities.UpdateOperationOutput<WidgetDetail> UpdateWidget(Widget widget)
        {
            return _manager.UpdateWidget(widget);
        }

        [HttpGet]
        [Route("GetAllWidgets")]
        public IEnumerable<WidgetDetail> GetAllWidgets()
        {
            return _manager.GetAllWidgets();
        }

        [HttpPost]
        [Route("GetFilteredWidgets")]
        public object GetFilteredWidgets(Vanrise.Entities.DataRetrievalInput<WidgetFilter> filter)
        {
            return GetWebResponse(filter, _manager.GetFilteredWidgets(filter), "Widgets");
        }

        [HttpGet]
        [Route("DeleteWidget")]
        public Vanrise.Entities.DeleteOperationOutput<WidgetDetail> DeleteWidget(int widgetId)
        {
            return _manager.DeleteWidget(widgetId);
        }

        [HttpGet]
        [Route("GetWidgetById")]
        public Widget GetWidgetById(int widgetId)
        {
            return _manager.GetWidgetById(widgetId);
        }

    }
}