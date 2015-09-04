using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.BusinessEntity.Business;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class SupplierCommissionController : Vanrise.Web.Base.BaseAPIController
    {
        public object GetSupplierCommissions(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            SupplierCommissionManager manager = new SupplierCommissionManager();
            return GetWebResponse(input, manager.GetSupplierCommissions(input));
        }

    }
}