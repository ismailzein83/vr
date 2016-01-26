using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Vanrise.Queueing.Entities;
using Vanrise.Queueing.Web.ModelMappers;
using Vanrise.Queueing.Web.Models;

namespace Vanrise.Queueing.Web.Controllers
{
    public class ExecutionFlowController : Vanrise.Web.Base.BaseAPIController
    {

        public List<QueueExecutionFlowDefinition> GetFilteredExecutionFlowDefinitions() 
        {

            return new List<QueueExecutionFlowDefinition>();
            
        }

    }
}