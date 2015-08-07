using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vanrise.Web
{
    public class LicenseCheckFilter : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //throw new Exception("License Key is expired. key is: ");
            base.OnAuthorization(actionContext);
        }
    }
}