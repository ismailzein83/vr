using Demo.Module.Data;
using Demo.Module.Entities.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class BuildingManager
    {
        public IDataRetrievalResult<BuildingDetails> GetFilteredBuildings(DataRetrievalInput<BuildingQuery> input)
        {
            var allBuildings = GetCachedBuildings();
            Func<Building, bool> filterExpression = (building) =>
            {
                if (input.Query.Name != null && !building.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allBuildings.ToBigResult(input, filterExpression, BuildingDetailMapper));
        }
        public IEnumerable<BuildingInfo> GetBuildingsInfo()
        {
            var allBuildings = GetCachedBuildings();
            Func<Building, bool> filterFunc = null;
            
                filterFunc = (building) =>
                {
                    return true;
                };
                IEnumerable<Building> filteredBuildings = (filterFunc != null) ? allBuildings.FindAllRecords(filterFunc) : allBuildings.MapRecords(u => u.Value);
                return filteredBuildings.MapRecords(BuildingInfoMapper).OrderBy(building => building.Name);
        }
        public Building GetBuildingById(int buildingId)
        {
            var allBuildings = GetCachedBuildings();
            return allBuildings.GetRecord(buildingId);
        }


        public InsertOperationOutput<BuildingDetails> AddBuilding(Building building)
        {
            IBuildingDataManager buildingDataManager = DemoModuleFactory.GetDataManager<IBuildingDataManager>();
            InsertOperationOutput<BuildingDetails> insertOperationOutput = new InsertOperationOutput<BuildingDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int buildingId = -1;
            bool insertActionSuccess = buildingDataManager.Insert(building, out buildingId);
            if (insertActionSuccess)
            {
                building.BuildingId = buildingId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = BuildingDetailMapper(building);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public UpdateOperationOutput<BuildingDetails> UpdateBuilding(Building building)
        {
            IBuildingDataManager buildingDataManager = DemoModuleFactory.GetDataManager<IBuildingDataManager>();
            UpdateOperationOutput<BuildingDetails> updateOperationOutput = new UpdateOperationOutput<BuildingDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = buildingDataManager.Update(building);
            if (updateActionSuccess)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BuildingDetailMapper(building);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        public DeleteOperationOutput<BuildingDetails> Delete(int Id)
        {
            IBuildingDataManager buildingDataManager = DemoModuleFactory.GetDataManager<IBuildingDataManager>();
            DeleteOperationOutput<BuildingDetails> deleteOperationOutput = new DeleteOperationOutput<BuildingDetails>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            bool deleteActionSuccess = buildingDataManager.Delete(Id);
            if (deleteActionSuccess)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }
            return deleteOperationOutput;
        }


        private Dictionary<int, Building> GetCachedBuildings()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject("GetCachedBuildings", () =>
                {
                    IBuildingDataManager buildingDataManager = DemoModuleFactory.GetDataManager<IBuildingDataManager>();
                    List<Building> buildings = buildingDataManager.GetBuildings();
                    return buildings.ToDictionary(building => building.BuildingId, building => building);
                });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBuildingDataManager buildingDataManager = DemoModuleFactory.GetDataManager<IBuildingDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return buildingDataManager.AreBuildingsUpdated(ref _updateHandle);
            }
        }
        public BuildingDetails BuildingDetailMapper(Building building)
        {
            BuildingDetails buildingDetails = new BuildingDetails();
            buildingDetails.Entity = building;
            return buildingDetails;
        }
        BuildingInfo BuildingInfoMapper(Building building)
        {
            BuildingInfo buildingInfo = new BuildingInfo();
            buildingInfo.BuildingId = building.BuildingId;
            buildingInfo.Name = building.Name;
            return buildingInfo;
        }
    }
}
