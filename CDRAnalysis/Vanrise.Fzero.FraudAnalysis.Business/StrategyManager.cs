using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class StrategyManager
    {

        private Dictionary<int, Strategy> GetCachedStrategies()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetStrategies",
               () =>
               {
                   IStrategyDataManager dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
                   IEnumerable<Strategy> strategies = dataManager.GetStrategies();
                   return strategies.ToDictionary(x => x.Id, x => x);
               });
        }

        public Vanrise.Entities.IDataRetrievalResult<StrategyDetail> GetFilteredStrategies(Vanrise.Entities.DataRetrievalInput<StrategyQuery> input)
        {
            var allStrategies = GetCachedStrategies();

            Func<Strategy, bool> filterExpression = (strategyObject) =>
                (input.Query.Name == null || strategyObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                && (input.Query.Description == null || strategyObject.Description.ToLower().Contains(input.Query.Description.ToLower()))
                && (input.Query.FromDate == null || strategyObject.LastUpdatedOn >= input.Query.FromDate)
                && (input.Query.ToDate == null || strategyObject.LastUpdatedOn < input.Query.ToDate)
                && (input.Query.PeriodIds == null || input.Query.PeriodIds.Contains((PeriodEnum)strategyObject.PeriodId))
                && (input.Query.UserIds == null || input.Query.UserIds.Contains(strategyObject.UserId))
                && (input.Query.Kinds == null || input.Query.Kinds.Contains(strategyObject.IsDefault ? StrategyKindEnum.SystemBuiltIn : StrategyKindEnum.UserDefined))
                && (input.Query.Statuses == null || input.Query.Statuses.Contains(strategyObject.IsEnabled ? StrategyStatusEnum.Enabled : StrategyStatusEnum.Disabled))
                ;

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allStrategies.ToBigResult(input, filterExpression, StrategyDetailMapper));
        }

        public Strategy GetStrategyById(int StrategyId)
        {
            var strategies = GetCachedStrategies();
            return strategies.GetRecord(StrategyId);
        }

        public IEnumerable<Strategy> GetStrategies()
        {
            var strategies = GetCachedStrategies();
            return strategies.Select(kvp => kvp.Value).ToList();
        }

        public IEnumerable<StrategyInfo> GetStrategiesInfo(int periodId, bool? isEnabled)
        {
            var strategies = GetCachedStrategies();

            Func<Strategy, bool> filterExpression = (strategyObject) =>
                  (periodId == 0 || periodId == strategyObject.PeriodId)
               && (isEnabled == null || isEnabled.Value == strategyObject.IsEnabled)
               ;
            return strategies.MapRecords(StrategyInfoMapper, filterExpression);
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
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetStrategies");
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
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetStrategies");
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IStrategyDataManager _dataManager = FraudDataManagerFactory.GetDataManager<IStrategyDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreStrategiesUpdated(ref _updateHandle);
            }
        }

        #region Private Members

        StrategyDetail StrategyDetailMapper(Strategy strategyObj)
        {
            UserManager userManager = new UserManager();
            User user = userManager.GetUserbyId(strategyObj.UserId);

            StrategyDetail strategyDetail = new StrategyDetail();
            strategyDetail.Entity = strategyObj;
            strategyDetail.Analyst = user.Name;
            strategyDetail.StrategyType = Vanrise.Common.Utilities.GetEnumDescription<PeriodEnum>((PeriodEnum)strategyObj.PeriodId);
            strategyDetail.StrategyKind = Vanrise.Common.Utilities.GetEnumDescription<StrategyKindEnum>((strategyObj.IsDefault ? StrategyKindEnum.SystemBuiltIn : StrategyKindEnum.UserDefined));
            return strategyDetail;
        }

        StrategyInfo StrategyInfoMapper(Strategy strategy)
        {
            StrategyInfo strategyInfo = new StrategyInfo();
            strategyInfo.Id = strategy.Id;
            strategyInfo.Name = strategy.Name;
            strategyInfo.PeriodId = strategy.PeriodId;
            strategyInfo.IsDefault = strategy.IsDefault;
            return strategyInfo;
        }

        # endregion

    }
}
