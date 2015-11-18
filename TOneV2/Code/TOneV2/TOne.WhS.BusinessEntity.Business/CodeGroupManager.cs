using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CodeGroupManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CodeGroupDetail> GetFilteredCodeGroups(Vanrise.Entities.DataRetrievalInput<CodeGroupQuery> input)
        {
            var allCodeGroups = GetCachedCodeGroups();

            Func<CodeGroup, bool> filterExpression = (prod) =>
                 (input.Query.Code == null || prod.Code.ToLower().Contains(input.Query.Code.ToLower()))
                  &&
                 (input.Query.CountriesIds == null || input.Query.CountriesIds.Count() == 0 || input.Query.CountriesIds.Contains(prod.CountryId)); 

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodeGroups.ToBigResult(input, filterExpression, CodeGroupDetailMapper));     
        }

        public CodeGroup GetMatchCodeGroup(string code)
        {
            CodeIterator<CodeGroup> codeIterator = GetCodeIterator();
            return codeIterator.GetLongestMatch(code);
        }

        private CodeIterator<CodeGroup> GetCodeIterator()
        {
            var cachedCodeGroups = GetCachedCodeGroups();
            return new CodeIterator<CodeGroup>(cachedCodeGroups.Values);
        }

        public IEnumerable<CodeGroup> GetAllCodeGroups()
        {
            var allCodeGroups = GetCachedCodeGroups();
            if (allCodeGroups == null)
                return null;

            return allCodeGroups.Values;
        }
        public CodeGroup GetCodeGroup(int codeGroupId)
        {
            var codeGroups = GetCachedCodeGroups();
            return codeGroups.GetRecord(codeGroupId);
        }
        public TOne.Entities.InsertOperationOutput<CodeGroupDetail> AddCodeGroup(CodeGroup codeGroup)
        {
            TOne.Entities.InsertOperationOutput<CodeGroupDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CodeGroupDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int coudeGroupId = -1;

            ICodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();
            bool insertActionSucc = dataManager.Insert(codeGroup, out coudeGroupId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                codeGroup.CodeGroupId = coudeGroupId;
                insertOperationOutput.InsertedObject = CodeGroupDetailMapper(codeGroup);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public TOne.Entities.UpdateOperationOutput<CodeGroupDetail> UpdateCodeGroup(CodeGroup codeGroup)
        {
            ICodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();

            bool updateActionSucc =  dataManager.Update(codeGroup);
            TOne.Entities.UpdateOperationOutput<CodeGroupDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CodeGroupDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject =  CodeGroupDetailMapper(codeGroup);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #region Private Members

        public Dictionary<int, CodeGroup> GetCachedCodeGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCodeGroups",
               () =>
               {
                   ICodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();
                   IEnumerable<CodeGroup> codegroups = dataManager.GetCodeGroups();
                   return codegroups.ToDictionary(cg => cg.CodeGroupId, cg => cg);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICodeGroupDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCodeGroupUpdated(ref _updateHandle);
            }
        }

        private CodeGroupDetail CodeGroupDetailMapper(CodeGroup codeGroup)
        {
            CodeGroupDetail codeGroupDetail = new CodeGroupDetail();

            codeGroupDetail.Entity = codeGroup;

            CountryManager manager = new CountryManager();
            if (codeGroup.CountryId != null)
            {
                int countryId = (int)codeGroup.CountryId;
                codeGroupDetail.CountryName = manager.GetCountry(countryId).Name;
            }

            return codeGroupDetail;
        }
        #endregion
    }
}
