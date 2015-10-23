using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    public class SellingNumberPlanController:BaseAPIController
    {
        [HttpGet]
        public List<SellingNumberPlan> GetSellingNumberPlans()
        {
            SellingNumberPlanManager manager = new SellingNumberPlanManager();
            return manager.GetSellingNumberPlans();
        }
    }
}