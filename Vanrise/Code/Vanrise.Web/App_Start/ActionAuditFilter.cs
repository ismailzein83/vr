﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Web;
//using System.Web.Mvc;
//using Vanrise.Common;
//using Vanrise.Common.Business;
//using Vanrise.Entities;

//namespace Vanrise.Web.App_Start
//{
//    public class ActionAuditFilter : System.Web.Http.Filters.ActionFilterAttribute
//    {
//        static VRActionAuditManager s_manager = new VRActionAuditManager();
//        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
//        {
//            try
//            {
//                string url = actionExecutedContext.Request.RequestUri.AbsolutePath.ToString();
//                var uri = actionExecutedContext.Request.RequestUri;
//                string host = uri.GetLeftPart(UriPartial.Authority);
//                new UserActionAuditManager().AddUserActionAudit(url, host);               
//            }
//            catch (Exception ex)
//            {
//                Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
//                throw;
//            }
//        }
//    }
//}