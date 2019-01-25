//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Http;
//using Vanrise.Common.Business;
//using Vanrise.Entities;
//using Vanrise.Web.Base;

//namespace Vanrise.Common.Web.Controllers
//{
//    [RoutePrefix(Constants.ROUTE_PREFIX + "HttpCallConnectionInterceptors")]
//    public class HttpCallConnectionInterceptorsController : BaseAPIController
//    {
//        [HttpGet]
//        [Route("GetHttpConnectionCallInterceptorTemplateConfigs")]
//        public City GetHttpConnectionCallInterceptorTemplateConfigs()
//        {
//            HttpConnectionCallInterceptorManager manager = new HttpConnectionCallInterceptorManager();
//            return manager.GetHttpConnectionCallInterceptorTemplateConfigs(cityHistoryId);
//        }
//    }
//}