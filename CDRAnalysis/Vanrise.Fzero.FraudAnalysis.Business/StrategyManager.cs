using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Security.Business;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyManager : IStrategyManager
    {
        #region Fields / Constructors

        UserManager _userManager;

        public StrategyManager()
        {
            _userManager = new UserManager();
        }

        #endregion

        #region Public Methods

        public IEnumerable<StrategyCriteriaConfig> GetStrategyCriteriaTemplateConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<StrategyCriteriaConfig>(StrategyCriteriaConfig.EXTENSION_TYPE);
        }
        public Vanrise.Entities.IDataRetrievalResult<StrategyDetail> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyQuery> input)
        {
            var cachedStrategies = GetCachedStrategies();

            Func<Strategy, bool> filterExpression = (strategyObject) =>
                (input.Query.Name == null || strategyObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                && (input.Query.Description == null || strategyObject.Description.ToLower().Contains(input.Query.Description.ToLower()))
                && (input.Query.FromDate == null || strategyObject.LastUpdatedOn >= input.Query.FromDate)
                && (input.Query.ToDate == null || strategyObject.LastUpdatedOn < input.Query.ToDate)
                && (input.Query.PeriodIds == null || input.Query.PeriodIds.Contains((PeriodEnum)strategyObject.Settings.PeriodId))
                && (input.Query.UserIds == null || input.Query.UserIds.Contains(strategyObject.UserId))
                && (input.Query.Kinds == null || input.Query.Kinds.Contains(strategyObject.Settings.IsDefault ? StrategyKind.SystemBuiltIn : StrategyKind.UserDefined))
                && (input.Query.Statuses == null || input.Query.Statuses.Contains(strategyObject.Settings.IsEnabled ? StrategyStatus.Enabled : StrategyStatus.Disabled));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedStrategies.ToBigResult(input, filterExpression, StrategyDetailMapper));
        }

        public Strategy GetStrategy(int strategyId)
        {
            var cachedStrategies = GetCachedStrategies();
            return cachedStrategies.GetRecord(strategyId);
        }

        public IEnumerable<Strategy> GetStrategies()
        {
            var strategies = GetCachedStrategies();
            return strategies.Select(kvp => kvp.Value).ToList();
        }


        public IEnumerable<StrategyInfo> GetStrategiesInfo(StrategyInfoFilter filter)
        {
            var cachedStrategies = GetCachedStrategies();
            Func<Strategy, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (strategy) => (filter.PeriodId == null || filter.PeriodId.Value == strategy.Settings.PeriodId) && (filter.IsEnabled == null || filter.IsEnabled.Value == strategy.Settings.IsEnabled);
            }
            return cachedStrategies.MapRecords(StrategyInfoMapper, filterExpression);
        }

        public IEnumerable<string> GetStrategyNames(IEnumerable<int> strategyIds)
        {
            var cachedStrategies = GetCachedStrategies();
            Func<Strategy, bool> filterExpression = null;
            if (strategyIds != null)
                filterExpression =  (strategy) => strategyIds.Contains(strategy.Id);
            return cachedStrategies.MapRecords(strategy => strategy.Name, filterExpression);
        }

        public InsertOperationOutput<StrategyDetail> AddStrategy(Strategy strategyObj)
        {
            InsertOperationOutput<StrategyDetail> insertOperationOutput = new InsertOperationOutput<StrategyDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int strategyId = -1;

            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            strategyObj.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            bool inserted = dataManager.AddStrategy(strategyObj, out strategyId);

            if (inserted)
            {
                strategyObj.Id = strategyId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = StrategyDetailMapper(strategyObj);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<StrategyDetail> UpdateStrategy(Strategy strategyObj)
        {
            UpdateOperationOutput<StrategyDetail> updateOperationOutput = new UpdateOperationOutput<StrategyDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            strategyObj.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            bool updated = dataManager.UpdateStrategy(strategyObj);

            if (updated)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = StrategyDetailMapper(strategyObj);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        
        #endregion

        #region Private Methods

        Dictionary<int, Strategy> GetCachedStrategies()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetStrategies",
               () =>
               {
                   IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
                   IEnumerable<Strategy> strategies = dataManager.GetStrategies();
                   return strategies.ToDictionary(x => x.Id, x => x);
               });
        }
        
        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IStrategyDataManager _dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreStrategiesUpdated(ref _updateHandle);
            }
        }
        
        #endregion

        #region Mappers

        StrategyDetail StrategyDetailMapper(Strategy strategy)
        {
            StrategyDetail strategyDetail = new StrategyDetail();
            strategyDetail.Entity = strategy;
            strategyDetail.Analyst = _userManager.GetUserName(strategy.UserId);
            strategyDetail.StrategyType = Vanrise.Common.Utilities.GetEnumDescription<PeriodEnum>((PeriodEnum)strategy.Settings.PeriodId);
            strategyDetail.StrategyKind = Vanrise.Common.Utilities.GetEnumDescription<StrategyKind>((strategy.Settings.IsDefault ? StrategyKind.SystemBuiltIn : StrategyKind.UserDefined));
            return strategyDetail;
        }

        StrategyInfo StrategyInfoMapper(Strategy strategy)
        {
            StrategyInfo strategyInfo = new StrategyInfo();
            strategyInfo.Id = strategy.Id;
            strategyInfo.Name = strategy.Name;
            strategyInfo.PeriodId = strategy.Settings.PeriodId;
            strategyInfo.IsDefault = strategy.Settings.IsDefault;
            return strategyInfo;
        }

        #endregion
    }
}
