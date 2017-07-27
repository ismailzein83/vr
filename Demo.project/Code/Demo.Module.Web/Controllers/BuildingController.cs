using Demo.Module.Business;
using Demo.Module.Entities.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Building")]
    public class BuildingController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredBuildings")]
        public object GetFilteredBuildings(DataRetrievalInput<BuildingQuery> input)
        {
            BuildingManager buildingManager = new BuildingManager();
            return GetWebResponse(input, buildingManager.GetFilteredBuildings(input));
        }

        [HttpGet]
        [Route("GetBuildingsInfo")]
        public IEnumerable<BuildingInfo> GetBuildingsInfo()
        {
            BuildingManager buildingManager = new BuildingManager();
            return buildingManager.GetBuildingsInfo();
        }

        [HttpGet]
        [Route("GetBuildingById")]
        public Building GetBuildingById(int buildingId)
        {
            BuildingManager buildingManager = new BuildingManager();
            return buildingManager.GetBuildingById(buildingId);
        }

        [HttpPost]
        [Route("AddBuilding")]
        public InsertOperationOutput<BuildingDetails> AddBuilding(Building building)
        {
            BuildingManager buildingManager = new BuildingManager();
            return buildingManager.AddBuilding(building);
        }

        [HttpPost]
        [Route("UpdateBuilding")]
        public UpdateOperationOutput<BuildingDetails> UpdateBuilding(Building building)
        {
            BuildingManager buildingManager = new BuildingManager();
            return buildingManager.UpdateBuilding(building);
        }

        [HttpGet]
        [Route("DeleteBuilding")]
        public DeleteOperationOutput<BuildingDetails> DeleteBuilding(int buildingId)
        {
            BuildingManager buildingManager = new BuildingManager();
            return buildingManager.Delete(buildingId);
        }
    }
}