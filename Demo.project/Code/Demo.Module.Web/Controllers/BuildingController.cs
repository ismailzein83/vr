using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Building")]
    [JSONWithTypeAttribute]
    public class Demo_Module_BuildingController : BaseAPIController
    {
        BuildingManager buildingManager = new BuildingManager();
        [HttpPost]
        [Route("GetFilteredBuildings")]
        public object GetFilteredBuildings(DataRetrievalInput<BuildingQuery> input)
        {
            return GetWebResponse(input, buildingManager.GetFilteredBuildings(input));
        }

        [HttpGet]
        [Route("GetBuildingById")]
        public Building GetBuildingById(long buildingId)
        {
            return buildingManager.GetBuildingById(buildingId);
        }

        [HttpPost]
        [Route("UpdateBuilding")]
        public UpdateOperationOutput<BuildingDetails> UpdateBuilding(Building building)
        {
            return buildingManager.UpdateBuilding(building);
        }

        [HttpPost]
        [Route("AddBuilding")]
        public InsertOperationOutput<BuildingDetails> AddBuilding(Building building)
        {
            return buildingManager.AddBuilding(building);
        }
    }
}
