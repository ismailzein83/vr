using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

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
                 (input.Query.CountriesIds == null || input.Query.CountriesIds.Count() == 0 || input.Query.CountriesIds.Contains(prod.CountryId)); ;

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCodeGroups.ToBigResult(input, filterExpression, SaleZoneDetailMapper));     
        }

        public IEnumerable<CodeGroup> GetAllCountries()
        {
            var allCodeGroups = GetCachedCodeGroups();
            if (allCodeGroups == null)
                return null;

            return allCodeGroups.Values;
        }
        public CodeGroup GetCountry(int codeGroupId)
        {
            var codeGroups = GetCachedCodeGroups();
            return codeGroups.GetRecord(codeGroupId);
        }
        public TOne.Entities.InsertOperationOutput<CodeGroup> AddCountry(CodeGroup codeGroup)
        {
            TOne.Entities.InsertOperationOutput<CodeGroup> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CodeGroup>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

           // int countryId = -1;

            //ICodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<ICountrytDataManager>();
            //bool insertActionSucc = dataManager.Insert(codeGroup, out coudeGroupId);
            //if (insertActionSucc)
            //{
            //    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
            //    country.CountryId = coudeGroupId;
            //    insertOperationOutput.InsertedObject = codeGroup;
            //}

            return insertOperationOutput;
        } 
        public TOne.Entities.UpdateOperationOutput<CodeGroup> UpdateCountry(CodeGroup codeGroup)
        {
            ICodeGroupDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeGroupDataManager>();

            //bool updateActionSucc =  dataManager.Update(codeGroup);
            TOne.Entities.UpdateOperationOutput<CodeGroup> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<CodeGroup>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            //updateOperationOutput.UpdatedObject = null;

            //if (updateActionSucc)
            //{
            //    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            //    updateOperationOutput.UpdatedObject = codeGroup;
            //}

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

        private CodeGroupDetail SaleZoneDetailMapper(CodeGroup codeGroup)
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
