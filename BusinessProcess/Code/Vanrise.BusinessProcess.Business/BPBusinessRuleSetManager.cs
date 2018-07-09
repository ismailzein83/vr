using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using System;
using Vanrise.Entities;
using System.Linq;

namespace Vanrise.BusinessProcess.Business
{
    public class BPBusinessRuleSetManager
    {
        #region public methods
        public Vanrise.Entities.IDataRetrievalResult<BPBusinessRuleSetDetail> GetFilteredBPBusinessRuleSets(Vanrise.Entities.DataRetrievalInput<BPBusinessRuleSetQuery> input)
        {
            var allBusinessRuleSets = GetCachedBPBusinessRuleSets();

            Func<BPBusinessRuleSet, bool> filterExpression = (prod) => (input.Query == null || input.Query.DefinitionsId == null || input.Query.DefinitionsId.Count == 0 || input.Query.DefinitionsId.Contains(prod.BPDefinitionId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allBusinessRuleSets.ToBigResult(input, filterExpression, BPBusinessRuleSetDetailMapper));
        }

        public BPBusinessRuleSet GetBusinessRuleSetsByID(int businessRuleSetId)
        {
            return GetCachedBPBusinessRuleSets().First(itm => itm.BPBusinessRuleSetId == businessRuleSetId);
        }
        public List<BPBusinessRuleSet> GetAllBusinessRuleSets ()
        {
            return GetCachedBPBusinessRuleSets();
        }
        public List<BPBusinessRuleSet> GetBusinessRuleSetsInfo(BPBusinessRuleSetInfoFilter filter)
        {
            var bpDefinitions = GetCachedBPBusinessRuleSets();

            if (filter == null)
                return bpDefinitions;

            Func<BPBusinessRuleSet, bool> filterExpression = (itm) => (!filter.BPDefinitionId.HasValue || filter.BPDefinitionId.Value == itm.BPDefinitionId);
            var bpBusinessRuleSets = bpDefinitions.FindAllRecords(filterExpression);

            if (bpBusinessRuleSets == null || bpBusinessRuleSets.Count() == 0)
                return null;

            if (!filter.CanBeParentOfRuleSetId.HasValue)
                return bpBusinessRuleSets.ToList();

            List<BPBusinessRuleSet> result = new List<BPBusinessRuleSet>();
            foreach (BPBusinessRuleSet bpBusinessRuleSet in bpBusinessRuleSets)
            {
                if (IsMatching(bpBusinessRuleSet, bpBusinessRuleSets, filter.CanBeParentOfRuleSetId.Value))
                {
                    result.Add(bpBusinessRuleSet);
                }
            }
            return result;
        }

        private bool IsMatching(BPBusinessRuleSet bpBusinessRuleSet, IEnumerable<BPBusinessRuleSet> bpBusinessRuleSets, int bpBusinessRuleSetId)
        {
            if (bpBusinessRuleSet.BPBusinessRuleSetId == bpBusinessRuleSetId)
                return false;

            if (!bpBusinessRuleSet.ParentId.HasValue)
                return true;

            if (bpBusinessRuleSet.ParentId.Value == bpBusinessRuleSetId)
                return false;

            BPBusinessRuleSet item = bpBusinessRuleSets.First(itm => itm.BPBusinessRuleSetId == bpBusinessRuleSet.ParentId.Value);
            return IsMatching(item, bpBusinessRuleSets, bpBusinessRuleSetId);
        }

        public Vanrise.Entities.InsertOperationOutput<BPBusinessRuleSetDetail> AddBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj)
        {
            InsertOperationOutput<BPBusinessRuleSetDetail> insertOperationOutput = new InsertOperationOutput<BPBusinessRuleSetDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int bpBusinessRuleSetId = -1;

            IBPBusinessRuleSetDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleSetDataManager>();
            bool insertActionSucc = dataManager.AddBusinessRuleSet(businessRuleSetObj, out bpBusinessRuleSetId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                businessRuleSetObj.BPBusinessRuleSetId = bpBusinessRuleSetId;
                insertOperationOutput.InsertedObject = BPBusinessRuleSetDetailMapper(businessRuleSetObj);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<BPBusinessRuleSetDetail> UpdateBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj)
        {
            IBPBusinessRuleSetDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleSetDataManager>();
            bool updateActionSucc = dataManager.UpdateBusinessRuleSet(businessRuleSetObj);
            UpdateOperationOutput<BPBusinessRuleSetDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<BPBusinessRuleSetDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BPBusinessRuleSetDetailMapper(businessRuleSetObj);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        #endregion

        #region private methods

        private List<BPBusinessRuleSet> GetCachedBPBusinessRuleSets()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBPBusinessRuleSets",
            () =>
            {
                IBPBusinessRuleSetDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleSetDataManager>();
                return dataManager.GetBPBusinessRuleSets();
            });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPBusinessRuleSetDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPBusinessRuleSetDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPBusinessRuleSetsUpdated(ref _updateHandle);
            }
        }

        private BPBusinessRuleSetDetail BPBusinessRuleSetDetailMapper(BPBusinessRuleSet bpBusinessRuleSet)
        {
            BPDefinitionManager bpDefinitionManager = new BPDefinitionManager();
            BPDefinition bpDefinition = bpDefinitionManager.GetBPDefinition(bpBusinessRuleSet.BPDefinitionId);

            BPBusinessRuleSet parentBusinessRuleSet = null;
            if (bpBusinessRuleSet.ParentId.HasValue)
                parentBusinessRuleSet = GetBusinessRuleSetsByID(bpBusinessRuleSet.ParentId.Value);

            return new BPBusinessRuleSetDetail()
            {
                Entity = bpBusinessRuleSet,
                BPDefinition = bpDefinition.Title,
                BPBusinessRuleSetParent = parentBusinessRuleSet != null ? parentBusinessRuleSet.Name : null,
            };
        }
        #endregion
    }
}