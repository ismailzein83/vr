using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class SupplierCommissionController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetSupplierCommissions(Vanrise.Entities.DataRetrievalInput<SupplierCommissionQuery> input)
        {
            SupplierCommissionManager manager = new SupplierCommissionManager();
            return GetWebResponse(input, manager.GetSupplierCommissions(input));
        }

    }
}