﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class TODController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetCutomerTODFromTempTable(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {

            TODManager manager = new TODManager();
            return GetWebResponse(input, manager.GetFilteredCustomerTOD(input));
        }

        [HttpPost]
        public object GetSupplierTODFromTempTable(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {

            TODManager manager = new TODManager();
            return GetWebResponse(input, manager.GetFilteredSupplierTOD(input));
        }
       
    }
}
