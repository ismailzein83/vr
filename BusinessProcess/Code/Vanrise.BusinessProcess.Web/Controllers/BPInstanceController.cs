using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Web.ModelMappers;
using Vanrise.BusinessProcess.Web.Models;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPInstance")]
    public class BPInstanceController : BaseAPIController
    {
        BPDefinitionManager _bpManager = new BPDefinitionManager();
        static BPInstanceManager s_instanceManager = new BPInstanceManager();

        [HttpPost]
        [Route("GetUpdated")]
        public BPInstanceUpdateOutput GetUpdated(BPInstanceUpdateInput input)
        {
            BPInstanceManager manager = new BPInstanceManager();
            byte[] maxTimeStamp = input.LastUpdateHandle;
            return manager.GetUpdated(ref maxTimeStamp, input.NbOfRows, input.DefinitionsId, input.ParentId, input.EntityIds);
        }

        [HttpPost]
        [Route("GetBeforeId")]
        public List<BPInstanceDetail> GetBeforeId(BPInstanceBeforeIdInput input)
        {
            BPInstanceManager manager = new BPInstanceManager();
            return manager.GetBeforeId(input);
        }

        [HttpPost]
        [Route("GetFilteredBPInstances")]
        public object GetFilteredBPInstances(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
        {
            BPInstanceManager manager = new BPInstanceManager();
            return GetWebResponse(input, manager.GetFilteredBPInstances(input));
        }

        [HttpGet]
        [Route("GetBPInstance")]
        public BPInstanceModel GetBPInstance(long id)
        {
            BPInstanceManager manager = new BPInstanceManager();
            return BPMappers.MapInstance(manager.GetBPInstance(id));
        }

        [HttpPost]
        [Route("CreateNewProcess")]
        public object CreateNewProcess(CreateProcessInput createProcessInput)
        {
            if (!_bpManager.DoesUserHaveStartNewInstanceAccess(Security.Entities.ContextFactory.GetContext().GetLoggedInUserId(),createProcessInput))
                return GetUnauthorizedResponse();

            BPInstanceManager manager = new BPInstanceManager();
            createProcessInput.InputArguments.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            return manager.CreateNewProcess(createProcessInput,true);
        }

        [HttpPost]
        [Route("HasRunningInstances")]
        public bool HasRunningInstances(HasRunningInstancesInput hasRunningInstancesInput)
        {
            BPInstanceManager manager = new BPInstanceManager();
            return manager.HasRunningInstances(hasRunningInstancesInput.definitionId, hasRunningInstancesInput.entityIds);
        }

        [HttpGet]
        [Route("GetBPDefinitionSummary")]
        public List<BPDefinitionSummary> GetBPDefinitionSummary()
        {
            BPInstanceManager manager = new BPInstanceManager();
            return manager.GetBPDefinitionSummary();
        }

        [HttpGet]
        [Route("DoesUserHaveCancelProcessAccess")]
        public bool DoesUserHaveCancelProcessAccess(long bpInstanceId)
        {
            return s_instanceManager.DoesUserHaveCancelAccess(Security.Entities.ContextFactory.GetContext().GetLoggedInUserId(), bpInstanceId);
        }

        [HttpGet]
        [Route("GetBPInstanceDefinitionDetail")]
        public BPInstanceDefinitionDetail GetBPInstanceDefinitionDetail(Guid bpDefinitionId, long bpInstanceId)
        {
            return s_instanceManager.GetBPInstanceDefinitionDetail(bpDefinitionId, bpInstanceId);
        }

        [HttpPost]
        [Route("CancelProcess")]
        public object CancelProcess(CancelProcessInput input)
        {
            if (!s_instanceManager.DoesUserHaveCancelAccess(Security.Entities.ContextFactory.GetContext().GetLoggedInUserId(), input.BPInstanceId))
                return GetUnauthorizedResponse();
            return s_instanceManager.CancelProcess(input.BPInstanceId);
        }

        [HttpGet]
        [Route("GetBPInstanceInsertHandlerConfigs")]
        public IEnumerable<BPInstanceInsertHandlerConfig> GetBPInstanceInsertHandlerConfigs()
        {
            return s_instanceManager.GetBPInstanceInsertHandlerConfigs();
        }
    }

    public class HasRunningInstancesInput
    {
        public Guid definitionId { get; set; }

        public List<string> entityIds { get; set; }
    }

    public class CancelProcessInput
    {
        public long BPInstanceId { get; set; }
    }
}