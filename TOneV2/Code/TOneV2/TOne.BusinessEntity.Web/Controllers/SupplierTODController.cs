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
    public class SupplierTODController : Vanrise.Web.Base.BaseAPIController
    {
    
        [HttpPost]
        public object GetSupplierTODFromTempTable(Vanrise.Entities.DataRetrievalInput<TODQuery> input)
        {

            SupplierTODManager manager = new SupplierTODManager();
            return GetWebResponse(input, manager.GetSupplierTODFiltered(input));
        }
       
    }
}
