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
    public partial class BusinessProcessController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public void StartInstance(string processName)
        {
            BPClient manager = new BPClient();
            object inputArguments = new object();
            manager.StartInstance(processName, inputArguments);
        }
    }
}