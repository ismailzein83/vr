using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
namespace NP.IVSwitch.Business
{
    public class TranslationRuleManager
    {
        #region Public Methods

        public IEnumerable<TranslationRuleInfo> GetTranslationRulesInfo(TranslationRuleFilter filter)
        {
            Func<TranslationRule, bool> filterExpression = null;

            return this.GetCachedTranslationRule().MapRecords(TranslationRuleInfoMapper, filterExpression).OrderBy(x => x.Name);
        }
        public TranslationRule GetTranslationRule(int translationRuleId)
        {
            Dictionary<int, TranslationRule> cachedTranslationRule = this.GetCachedTranslationRule();
            return cachedTranslationRule.GetRecord(translationRuleId);
        }

        public IDataRetrievalResult<TranslationRuleDetail> GetFilteredTranslationRules(DataRetrievalInput<TranslationRuleQuery> input)
        {
            var allTranslationRules = this.GetCachedTranslationRule();
            Func<TranslationRule, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allTranslationRules.ToBigResult(input, filterExpression, TranslationRuleDetailMapper));
        }

        public InsertOperationOutput<TranslationRuleDetail> AddTranslationRule(TranslationRule translationRuleItem)
        {
            var insertOperationOutput = new InsertOperationOutput<TranslationRuleDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };
            ITranslationRuleDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ITranslationRuleDataManager>();
            Helper.SetSwitchConfig(dataManager);
            int translationRuleId;

            if (dataManager.Insert(translationRuleItem, out  translationRuleId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = TranslationRuleDetailMapper(GetTranslationRule(translationRuleId));
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public UpdateOperationOutput<TranslationRuleDetail> UpdateTranslationRule(TranslationRule translationRuleItem)
        {
            var updateOperationOutput = new UpdateOperationOutput<TranslationRuleDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            ITranslationRuleDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ITranslationRuleDataManager>();
            Helper.SetSwitchConfig(dataManager);
            if (dataManager.Update(translationRuleItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = TranslationRuleDetailMapper(GetTranslationRule(translationRuleItem.TranslationRuleId));
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
            ITranslationRuleDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<ITranslationRuleDataManager>();
            //    object _updateHandle;
            public DateTime lastCheckTime { get; set; }
            protected override bool IsTimeExpirable { get { return true; } }


        }
        #endregion

        #region Private Methods

        Dictionary<int, TranslationRule> GetCachedTranslationRule()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetTranslationRule",
               () =>
               {
                   ITranslationRuleDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ITranslationRuleDataManager>();
                   Helper.SetSwitchConfig(dataManager);
                   return dataManager.GetTranslationRules().ToDictionary(x => x.TranslationRuleId, x => x);
               });
        }

        #endregion


        #region Mappers

        public TranslationRuleDetail TranslationRuleDetailMapper(TranslationRule TranslationRule)
        {
            TranslationRuleDetail TranslationRuleDetail = new TranslationRuleDetail()
            {
                Entity = TranslationRule
            };
            return TranslationRuleDetail;
        }

        public TranslationRuleInfo TranslationRuleInfoMapper(TranslationRule translationRule)
        {
            TranslationRuleInfo translationRuleInfo = new TranslationRuleInfo()
            {
                TranslationRuleId = translationRule.TranslationRuleId,
                Name = translationRule.Name,

            };
            return translationRuleInfo;
        }

        #endregion

    }
}
