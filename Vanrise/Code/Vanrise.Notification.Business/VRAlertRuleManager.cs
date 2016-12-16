using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Notification.Data;
using Vanrise.Common.Business;

namespace Vanrise.Notification.Business
{
    public class VRAlertRuleManager
    {
        #region Public Methods
        public List<VRAlertRule> GetActiveRules(Guid ruleTypeId)
        {
            return GetCachedRulesByType().GetRecord(ruleTypeId);
        }

        public VRAlertRule GetVRAlertRule(long vrAlertRuleId)
        {
            Dictionary<long, VRAlertRule> cachedVRAlertRules = this.GetCachedVRAlertRules();
            return cachedVRAlertRules.GetRecord(vrAlertRuleId);
        }

        public IDataRetrievalResult<VRAlertRuleDetail> GetFilteredVRAlertRules(DataRetrievalInput<VRAlertRuleQuery> input)
        {
            var allVRAlertRules = this.GetCachedVRAlertRules();
            Func<VRAlertRule, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRAlertRules.ToBigResult(input, filterExpression, VRAlertRuleDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<VRAlertRuleDetail> AddVRAlertRule(VRAlertRule vrAlertRuleItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRAlertRuleDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long vrAlertRuleId = -1;

            IVRAlertRuleDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleDataManager>();

            if (dataManager.Insert(vrAlertRuleItem, out vrAlertRuleId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                vrAlertRuleItem.VRAlertRuleId = vrAlertRuleId;
                insertOperationOutput.InsertedObject = VRAlertRuleDetailMapper(vrAlertRuleItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRAlertRuleDetail> UpdateVRAlertRule(VRAlertRule vrAlertRuleItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRAlertRuleDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRAlertRuleDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleDataManager>();

            if (dataManager.Update(vrAlertRuleItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRAlertRuleDetailMapper(this.GetVRAlertRule(vrAlertRuleItem.VRAlertRuleId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        //public IEnumerable<VRAlertRuleInfo> GetVRAlertRulesInfo(VRAlertRuleFilter filter)
        //{
        //    Func<VRAlertRule, bool> filterExpression = null;

        //    return this.GetCachedVRAlertRules().MapRecords(VRAlertRuleInfoMapper, filterExpression).OrderBy(x => x.Name);
        //}

        #endregion


        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRAlertRuleDataManager _dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRAlertRuleUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Private Methods

        private Dictionary<Guid, List<VRAlertRule>> GetCachedRulesByType()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRAlertRulesByTypeId",
               () =>
               {
                   return GetCachedVRAlertRules().Values.GroupBy(s => s.RuleTypeId).ToDictionary(x => x.Key, v => v.ToList());
               });
        }

        Dictionary<long, VRAlertRule> GetCachedVRAlertRules()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRAlertRules",
               () =>
               {
                   IVRAlertRuleDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRAlertRuleDataManager>();
                   return dataManager.GetVRAlertRules().ToDictionary(x => x.VRAlertRuleId, x => x);
               });
        }

        #endregion


        #region Mappers

        public VRAlertRuleDetail VRAlertRuleDetailMapper(VRAlertRule vrAlertRule)
        {
            VRAlertRuleDetail vrAlertRuleDetail = new VRAlertRuleDetail()
            {
                Entity = vrAlertRule
            };
            return vrAlertRuleDetail;
        }

        //public VRAlertRuleInfo VRAlertRuleInfoMapper(VRAlertRule vrAlertRule)
        //{
        //    VRAlertRuleInfo vrAlertRuleInfo = new VRAlertRuleInfo()
        //    {
        //        VRAlertRuleId = vrAlertRule.VRAlertRuleId,
        //        Name = vrAlertRule.Name
        //    };
        //    return vrAlertRuleInfo;
        //}

        #endregion
    }
}
