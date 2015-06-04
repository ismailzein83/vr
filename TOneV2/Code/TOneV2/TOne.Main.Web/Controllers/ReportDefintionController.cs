using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Business;
using TOne.Entities;

namespace TOne.Main.Web.Controllers
{
    public class ReportDefintionController : ApiController
    {
        [HttpGet]
        public List<RDLCReportDefinition> GetAllRDLCReportDefinition()
        {
            ReportDefinitionManager manager = new ReportDefinitionManager();
            return manager.GetAllRDLCReportDefinition();
        }

        
    }
}