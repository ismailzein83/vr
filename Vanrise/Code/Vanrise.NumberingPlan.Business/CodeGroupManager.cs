using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.NumberingPlan.Data;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class CodeGroupManager
    {
        #region ctor/Local Variables
        private readonly CountryManager _countryManager;
        public CodeGroupManager()
        {
            _countryManager = new CountryManager();
        }
        #endregion

        #region Public Methods
      
        public CodeGroup GetMatchCodeGroup(string code)
        {
            CodeIterator<CodeGroup> codeIterator = GetCodeIterator();
            return codeIterator.GetLongestMatch(code);
        }
      
        #endregion


        #region Private Members
        private Dictionary<int, CodeGroup> GetCachedCodeGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCodeGroups",
               () =>
               {
                   ICodeGroupDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodeGroupDataManager>();
                   IEnumerable<CodeGroup> codegroups = dataManager.GetCodeGroups();
                   return codegroups.ToDictionary(cg => cg.CodeGroupId, cg => cg);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICodeGroupDataManager _dataManager = CodePrepDataManagerFactory.GetDataManager<ICodeGroupDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCodeGroupUpdated(ref _updateHandle);
            }
        }
        private CodeIterator<CodeGroup> GetCodeIterator()
        {
            var cachedCodeGroups = GetCachedCodeGroups();
            return new CodeIterator<CodeGroup>(cachedCodeGroups.Values);
        }
        #endregion

   
    }
}
