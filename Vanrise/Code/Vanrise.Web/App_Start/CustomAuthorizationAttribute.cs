using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vanrise.Web.App_Start
{
    public class CustomAuthorizationAttribute : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        public bool isAllowed { get; set; }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            
            base.OnAuthorization(actionContext);
        }
    }
}