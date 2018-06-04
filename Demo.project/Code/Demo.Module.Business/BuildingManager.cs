using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace Demo.Module.Business
{
   public class BuildingManager
    {

        #region Public Methods
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


        public InsertOperationOutput<BuildingDetails> AddBuilding(Building building)
        {
            IBuildingDataManager buildingDataManager = DemoModuleFactory.GetDataManager<IBuildingDataManager>();
            InsertOperationOutput<BuildingDetails> insertOperationOutput = new InsertOperationOutput<BuildingDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long buildingId = -1;

            bool insertActionSuccess = buildingDataManager.Insert(building, out buildingId);
            if (insertActionSuccess)
            {
                building.BuildingId = buildingId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = BuildingDetailMapper(building);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public Building GetBuildingById(long buildingId)
        {
            var allBuildings = GetCachedBuildings();
            return allBuildings.GetRecord(buildingId);
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
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BuildingDetailMapper(building);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBuildingDataManager buildingDataManager = DemoModuleFactory.GetDataManager<IBuildingDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return buildingDataManager.AreCompaniesUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<long, Building> GetCachedBuildings()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedBuildings", () =>
               {
                   IBuildingDataManager buildingDataManager = DemoModuleFactory.GetDataManager<IBuildingDataManager>();
                   List<Building> buildings = buildingDataManager.GetBuildings();
                   return buildings.ToDictionary(building => building.BuildingId, building => building);
               });
        }
        #endregion
        
        #region Mappers
        public BuildingDetails BuildingDetailMapper(Building building)
        {
            return new BuildingDetails
            {
                Name = building.Name,
                BuildingId = building.BuildingId
            };
        }
        #endregion 

    }
}
