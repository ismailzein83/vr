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
        public Vanrise.Entities.InsertOperationOutput<Widget> SaveWidget(Widget widget)
        {
            WidgetsManager manager = new WidgetsManager();
            return manager.SaveWidget(widget);
        }
         [HttpGet]
        public List<Widget> GetAllWidgets()
        {
            WidgetsManager manager = new WidgetsManager();
            return manager.GetAllWidgets();
        }
    }
}