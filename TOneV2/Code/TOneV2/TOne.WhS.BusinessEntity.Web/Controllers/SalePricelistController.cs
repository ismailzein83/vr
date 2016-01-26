﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Web;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SalePricelist")]
    public class WhS_BE_SalePricelistController : Vanrise.Web.Base.BaseAPIController 
    {
        [HttpPost]
        [Route("GetFilteredSalePriceLists")]
        public object GetFilteredSalePriceLists(Vanrise.Entities.DataRetrievalInput<SalePriceListQuery> input)
        {
            SalePriceListManager manager = new SalePriceListManager();
            return GetWebResponse(input,manager.GetFilteredPricelists(input));
        }
        
       
    }
}
