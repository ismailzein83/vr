using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOne.Analytics.Web.Controllers
{
    public class BillingStatisticsController : Vanrise.Web.Base.BaseAPIController
    {
        public string GetTest(string name) {
            return "Sara"+name;
        }
    }
}