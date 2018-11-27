using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Reflection;

namespace Vanrise.Web.Base
{
    public class VRControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetCustomAttribute<RouteAttribute>() == null)
            {
                var customNameAttribute = controller.ControllerType.GetCustomAttribute<System.Web.Http.RoutePrefixAttribute>();


                if (customNameAttribute?.Prefix != null)
                {
                    controller.Selectors.Clear();
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"{customNameAttribute.Prefix}/[action]")),
                    });
                }
            }
        }
    }
}
