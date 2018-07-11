using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities;
using Demo.Module.Business;
using Vanrise.Entities;


namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Payment")]
    [JSONWithTypeAttribute]
    public class PaymentController : BaseAPIController
    {
        PaymentManager paymentManager = new PaymentManager();

        [HttpGet]
        [Route("GetPayments")]
        public List<Payment> GetPayments()
        {
            return paymentManager.GetPayments();

        }
       
    }
}
