using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class CallClassManager
    {
        public IEnumerable<CallClass> GetClasses()
        {
            var callClasses = GetCachedCallClasses();
            return callClasses.Values;
        }

        public static CallClass GetCallClassByDesc(string description)
        {
            var callClasses = GetCachedCallClasses();
            return callClasses.FindRecord(x => x.Value.Description == description).Value;
        }

        public static CallClass GetCallClassById(int id)
        {
            var callClasses = GetCachedCallClasses();
            return callClasses.GetRecord(id);
        }


        private static Dictionary<int, CallClass> GetCachedCallClasses()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCallClasses",
               () =>
               {
                   IClassDataManager dataManager = FraudDataManagerFactory.GetDataManager<IClassDataManager>();
                   IEnumerable<CallClass> callClasses = dataManager.GetCallClasses();
                   return callClasses.ToDictionary(kvp => kvp.Id, kvp => kvp);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IClassDataManager _dataManager = FraudDataManagerFactory.GetDataManager<IClassDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCallClassesUpdated(ref _updateHandle);
            }
        }





    }
}
