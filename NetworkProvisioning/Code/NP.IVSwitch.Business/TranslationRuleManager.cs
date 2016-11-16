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

        public Vanrise.Entities.InsertOperationOutput<TranslationRuleDetail> AddTranslationRule(TranslationRule translationRuleItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<TranslationRuleDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ITranslationRuleDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ITranslationRuleDataManager>();

            int translationRuleId = -1;

            if (dataManager.Insert(translationRuleItem, out  translationRuleId))
            {
               Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = TranslationRuleDetailMapper(this.GetTranslationRule(translationRuleId));
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<TranslationRuleDetail> UpdateTranslationRule(TranslationRule translationRuleItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<TranslationRuleDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ITranslationRuleDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ITranslationRuleDataManager>();

            if (dataManager.Update(translationRuleItem))
            {
              Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = TranslationRuleDetailMapper(this.GetTranslationRule(translationRuleItem.TranslationRuleId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
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
