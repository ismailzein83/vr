using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PaymentConfigs")]
    public class Demo_Module_ExtensionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetPaymentTypeTemplateConfigs")]
        public IEnumerable<PaymentConfig> GetPaymentTypeTemplateConfigs()
        {
            PaymentConfigsManager paymentConfigsManager = new PaymentConfigsManager();
            return paymentConfigsManager.GetPaymentTemplateConfigs();
        }

    }
}