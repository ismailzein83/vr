using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Data;
using Demo.Module.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;
using System.ComponentModel;
namespace Demo.Module.Business
{
    public class UnitTypeManager
    {

        public IEnumerable<UnitType> GetUnitTypesInfo()
        {
            IEnumerable<UnitType> allUnits = GetCachedUnitTypes().Values;
            return allUnits;
        }
        public UnitType GetUnitType(int unitTypeId)
        {
            return GetCachedUnitTypes().GetRecord(unitTypeId);
        }
        
        private Dictionary<int, UnitType> GetCachedUnitTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetUnitTypes",
               () =>
               {
                   IUnitTypeDataManager dataManager = DemoModuleDataManagerFactory.GetDataManager<IUnitTypeDataManager>();
                   IEnumerable<UnitType> unitTypes = dataManager.GetAllUnitTypes();
                   return unitTypes.ToDictionary(st => st.UnitTypeId, st => st);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IUnitTypeDataManager _dataManager = DemoModuleDataManagerFactory.GetDataManager<IUnitTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreUnitTypeUpdated(ref _updateHandle);
            }
        }
        
    }
}
