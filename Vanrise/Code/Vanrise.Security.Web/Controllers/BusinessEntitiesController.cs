﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class BusinessEntitiesController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public IEnumerable<BusinessEntityNode> GetEntityNodes()
        {
            BusinessEntityManager manager = new BusinessEntityManager();
            return manager.GetEntityNodes();
        }
    }
}