using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace Vanrise.Web
{
    public class VRHttpActionSelector : System.Web.Http.Controllers.ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            VRHttpControllerDescriptor vrControllerDescriptor = controllerContext.ControllerDescriptor as VRHttpControllerDescriptor;
            if (vrControllerDescriptor != null)
            {
                return vrControllerDescriptor.GetActionDescriptor(ParseRequestActionName(controllerContext.Request), this);
            }
            else
            {
                var actionDescriptor = base.SelectAction(controllerContext);
                return actionDescriptor;
            }
        }

        string ParseRequestActionName(System.Net.Http.HttpRequestMessage request)
        {
            string absolutePath = request.RequestUri.AbsolutePath;
            var pathWithoutAPI = absolutePath.Substring(absolutePath.IndexOf("api/") + 4);
            string[] parts = pathWithoutAPI.Split('/');
            if (parts.Length < 3)
                throw new Exception(String.Format("URL parts count is less than 3. Count is '{0}'", parts.Length));
            return parts[2];
        }
    }
}