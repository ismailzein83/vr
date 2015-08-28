using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CustomerTODController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetCutomerTODFromTempTable(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {

            CustomerTODManager manager = new CustomerTODManager();
            return GetWebResponse(input, manager.GetCustomerTODFiltered(input));
        }

    }
}
