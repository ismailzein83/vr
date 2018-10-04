using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "ProcessSynchronisation")]
    public class ProcessSynchronisationController: BaseAPIController
    {
        ProcessSynchronisationManager manager = new ProcessSynchronisationManager();

        [HttpPost]
        [Route("GetFilteredProcessesSynchronisations")]
        public object GetFilteredProcessSynchronisations(Vanrise.Entities.DataRetrievalInput<ProcessSynchronisationQuery> input)
        {
            return GetWebResponse(input, manager.GetFilteredProcessesSynchronisations(input));
        }

        [HttpPost]
        [Route("AddProcessSynchronisation")]
        public Vanrise.Entities.InsertOperationOutput<ProcessSynchronisationDetail> AddProcessSynchronisation(ProcessSynchronisationToAdd processSynchronisationToAdd)
        {
            return manager.AddProcessSynchronisation(processSynchronisationToAdd);
        }

        [HttpPost]
        [Route("UpdateProcessSynchronisation")]
        public Vanrise.Entities.UpdateOperationOutput<ProcessSynchronisationDetail> UpdateProcessSynchronisation(ProcessSynchronisationToUpdate processSynchronisationToUpdate)
        {
            return manager.UpdateProcessSynchronisation(processSynchronisationToUpdate);
        }

        [HttpGet]
        [Route("GetProcessSynchronisations")]
        public IEnumerable<ProcessSynchronisation> GetProcessSynchronisations()
        {
            return manager.GetProcessSynchronisations();
        }

        [HttpGet]
        [Route("GetProcessSynchronisation")]
        public ProcessSynchronisation GetProcessSynchronisation(Guid processSynchronisationId)
        {
            return manager.GetProcessSynchronisation(processSynchronisationId);
        }
       
    }
}