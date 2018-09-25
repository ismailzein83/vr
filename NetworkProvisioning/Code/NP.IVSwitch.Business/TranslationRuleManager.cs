using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Caching;
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

        public TranslationRule GetTranslationRuleHistoryDetailbyHistoryId(int translationRuleHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var translationRule = s_vrObjectTrackingManager.GetObjectDetailById(translationRuleHistoryId);
            return translationRule.CastWithValidate<TranslationRule>("TranslationRule : historyId ", translationRuleHistoryId);
        }

        public TranslationRule GetTranslationRule(int translationRuleId, bool isViewedFromUI)
        {
            Dictionary<int, TranslationRule> cachedTranslationRule = this.GetCachedTranslationRule();
            var translationRule = cachedTranslationRule.GetRecord(translationRuleId);
            if (translationRule != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(TranslationRuleLoggableEntity.Instance, translationRule);
            return translationRule;
        }

        public TranslationRule GetTranslationRule(int translationRuleId)
        {
            return GetTranslationRule(translationRuleId, false);
        }

        public string GetTranslationRuleName(int Id)
        {
            var translationRule = this.GetTranslationRule(Id);

            return translationRule != null ? translationRule.Name : null;
        }

        public IDataRetrievalResult<TranslationRuleDetail> GetFilteredTranslationRules(DataRetrievalInput<TranslationRuleQuery> input)
        {
            var allTranslationRules = this.GetCachedTranslationRule();
            Func<TranslationRule, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            VRActionLogger.Current.LogGetFilteredAction(TranslationRuleLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allTranslationRules.ToBigResult(input, filterExpression, TranslationRuleDetailMapper));
        }

        public IDataRetrievalResult<CLIPatternDetail> GetFilteredCLIPatterns(DataRetrievalInput<CLIPatternQuery> input)
        {
            var allTranslationRules = this.GetCachedTranslationRule();
            var translationRule = allTranslationRules.GetRecord(input.Query.TranslationRuleId);
            translationRule.ThrowIfNull("translationRule", input.Query.TranslationRuleId);
            Func<CLIPatternDetail, bool> filterExpression = (x) =>
            {
                return true;
            };
            List<CLIPatternDetail> cliPatternDetails = new List<CLIPatternDetail>();
            if (translationRule.PoolBasedCLISettings != null && translationRule.PoolBasedCLISettings.CLIPatterns!=null && translationRule.PoolBasedCLISettings.CLIPatterns.Count>0)
            {
                foreach(var cliPatternItem in translationRule.PoolBasedCLISettings.CLIPatterns){
                    cliPatternDetails.Add(new CLIPatternDetail()
                    {
                        CLIPattern = cliPatternItem,
                        Destination = translationRule.PoolBasedCLISettings.Destination,
                        DisplayName = translationRule.PoolBasedCLISettings.DisplayName,
                        Prefix = translationRule.PoolBasedCLISettings.Prefix,
                        RandMax = translationRule.PoolBasedCLISettings.RandMax.Value == 0 ? (int?)null : translationRule.PoolBasedCLISettings.RandMax.Value,
                        RandMin = translationRule.PoolBasedCLISettings.RandMin.Value == -1 ? (int?)null : translationRule.PoolBasedCLISettings.RandMin.Value
                    });
                }
            }
            VRActionLogger.Current.LogGetFilteredAction(TranslationRuleLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cliPatternDetails.ToBigResult<CLIPatternDetail>(input, filterExpression));
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
                translationRuleItem.TranslationRuleId = translationRuleId;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(TranslationRuleLoggableEntity.Instance, translationRuleItem);
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
                VRActionLogger.Current.TrackAndLogObjectUpdated(TranslationRuleLoggableEntity.Instance, translationRuleItem);
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = TranslationRuleDetailMapper(GetTranslationRule(translationRuleItem.TranslationRuleId));
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public DeleteOperationOutput<TranslationRuleDetail> DeleteTranslationRule(int translationRuleId)
        {
            var deleteOperationOutput = new DeleteOperationOutput<TranslationRuleDetail>
            {
                Result = DeleteOperationResult.Failed,
            };
            ITranslationRuleDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ITranslationRuleDataManager>();
            Helper.SetSwitchConfig(dataManager);
            if (dataManager.Delete(translationRuleId))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }
            return deleteOperationOutput;
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

        private class  TranslationRuleLoggableEntity : VRLoggableEntityBase
        {
            public static TranslationRuleLoggableEntity Instance = new TranslationRuleLoggableEntity();

            private TranslationRuleLoggableEntity()
            {

            }

            static TranslationRuleManager translationRuleManager = new TranslationRuleManager();

            public override string EntityUniqueName
            {
                get { return "NP_IVSwitch_TranslationRule"; }
            }

            public override string ModuleName
            {
                get { return "IVSwitch"; }
            }

            public override string EntityDisplayName
            {
                get { return "Translation Rule"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "NP_IVSwitch_TranslationRule_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                TranslationRule translationRule = context.Object.CastWithValidate<TranslationRule>("context.Object");
                return translationRule.TranslationRuleId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                TranslationRule translationRule = context.Object.CastWithValidate<TranslationRule>("context.Object");
                return translationRuleManager.GetTranslationRuleName(translationRule.TranslationRuleId);
            }
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
                TranslationRuleId = TranslationRule.TranslationRuleId,
                Name = TranslationRule.Name,
                DNISPattern = GetDNISPAttern(TranslationRule),
                CLIPattern = GetCLIPattern(TranslationRule),
                CLIType = TranslationRule.CLIType,
                EngineType = Utilities.GetEnumDescription<EngineType>(TranslationRule.EngineType),
                CreationDate = TranslationRule.CreationDate
            };
            return TranslationRuleDetail;
        }

        private string GetCLIPattern(TranslationRule translationRule)
        {
            string cliPattern="";
            if (translationRule.FixedCLISettings != null)
            {
                cliPattern = translationRule.FixedCLISettings.CLIPattern;
                if (translationRule.FixedCLISettings.CLIPatternSign.HasValue)
                {
                    var sign = translationRule.FixedCLISettings.CLIPatternSign.Value == PrefixSign.Plus ? "+" : "-";
                    cliPattern = string.Concat(sign, cliPattern);
                }
            }
            if (translationRule.PoolBasedCLISettings != null)
            {
                cliPattern = String.Concat("*", translationRule.PoolBasedCLISettings.PoolId);
            }
            return cliPattern;
        }

        private string GetDNISPAttern(TranslationRule translationRule)
        {
            string dnisPattern = "";
            dnisPattern = translationRule.DNISPattern;
            if (translationRule.DNISPatternSign.HasValue && !String.IsNullOrEmpty(dnisPattern))
            {
                var sign = translationRule.DNISPatternSign.Value == PrefixSign.Plus ? "+" : "-";
                dnisPattern = string.Concat(sign, dnisPattern);
            }
            return dnisPattern;
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
