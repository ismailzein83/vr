using Demo.Module.Business;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DestinationGroup")]
    public class Demo_DestinationGroupController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredDestinationGroups")]
        public object GetFilteredDestinationGroups(Vanrise.Entities.DataRetrievalInput<DestinationGroupQuery> input)
        {
            DestinationGroupManager manager = new DestinationGroupManager();
            return GetWebResponse(input, manager.GetFilteredDestinationGroups(input));
        }

        [HttpGet]
        [Route("GetDestinationGroup")]
        public DestinationGroup GetDestinationGroup(int destinationGroupId)
        {
            DestinationGroupManager manager = new DestinationGroupManager();
            return manager.GetDestinationGroup(destinationGroupId);
        }

        [HttpPost]
        [Route("AddDestinationGroup")]
        public Vanrise.Entities.InsertOperationOutput<DestinationGroupDetail> AddDestinationGroup(DestinationGroup destinationGroup)
        {
            DestinationGroupManager manager = new DestinationGroupManager();
            return manager.AddDestinationGroup(destinationGroup);
        }
        [HttpPost]
        [Route("UpdateDestinationGroup")]
        public Vanrise.Entities.UpdateOperationOutput<DestinationGroupDetail> UpdateDestinationGroup(DestinationGroup destinationGroup)
        {
            DestinationGroupManager manager = new DestinationGroupManager();
            return manager.UpdateDestinationGroup(destinationGroup);
        }


        [HttpGet]
        [Route("GetGroupTypeTemplates")]
        public List<TemplateConfig> GetGroupTypeTemplates()
        {
            DestinationGroupManager manager = new DestinationGroupManager();
            return manager.GetGroupTypeTemplates();
        }

        [HttpGet]
        [Route("GetDestinationGroupsInfo")]
        public IEnumerable<DestinationGroupInfo> GetDestinationGroupsInfo()
        {
            DestinationGroupManager manager = new DestinationGroupManager();
            return manager.GetDestinationGroupsInfo();
        }



    }
}