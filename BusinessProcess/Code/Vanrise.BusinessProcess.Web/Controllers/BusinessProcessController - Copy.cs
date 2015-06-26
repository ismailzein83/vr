using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Web.ModelMappers;
using Vanrise.BusinessProcess.Web.Models;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    
    public partial  class BusinessProcessController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public BPDefinition GetDefinition(int ID)
        {
            BPClient manager = new BPClient();
            return manager.GetDefinition(ID);
        }
    }

}