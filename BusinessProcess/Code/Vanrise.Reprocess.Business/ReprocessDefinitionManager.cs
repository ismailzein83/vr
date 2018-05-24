using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Reprocess.Data;
using Vanrise.Reprocess.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;


namespace Vanrise.Reprocess.Business
{
    public class ReprocessDefinitionManager
    {
        #region Public Methods

        public IDataRetrievalResult<ReprocessDefinitionDetail> GetFilteredReprocessDefinitions(DataRetrievalInput<ReprocessDefinitionQuery> input)
        {
            var allReprocessDefinitions = this.GetCachedReprocessDefinitions();
            Func<ReprocessDefinition, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            VRActionLogger.Current.LogGetFilteredAction(ReprocessDefinitionLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allReprocessDefinitions.ToBigResult(input, filterExpression, ReprocessDefinitionDetailMapper));
        }

        public ReprocessDefinition GetReprocessDefinition(Guid reprocessDefinitionId)
        {
            Dictionary<Guid, ReprocessDefinition> cachedReprocessDefinitions = this.GetCachedReprocessDefinitions();
            return cachedReprocessDefinitions.GetRecord(reprocessDefinitionId);
        }

        public string GetReprocessDefinitionName(ReprocessDefinition reprocessDefinition)
        {
            return (reprocessDefinition != null) ? reprocessDefinition.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<ReprocessDefinitionDetail> AddReprocessDefinition(ReprocessDefinition reprocessDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ReprocessDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            reprocessDefinitionItem.ReprocessDefinitionId = Guid.NewGuid();
            IReprocessDefinitionDataManager dataManager = ReprocessDataManagerFactory.GetDataManager<IReprocessDefinitionDataManager>();

            if (dataManager.Insert(reprocessDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(ReprocessDefinitionLoggableEntity.Instance, reprocessDefinitionItem);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ReprocessDefinitionDetailMapper(reprocessDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<ReprocessDefinitionDetail> UpdateReprocessDefinition(ReprocessDefinition reprocessDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ReprocessDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IReprocessDefinitionDataManager dataManager = ReprocessDataManagerFactory.GetDataManager<IReprocessDefinitionDataManager>();

            if (dataManager.Update(reprocessDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(ReprocessDefinitionLoggableEntity.Instance, reprocessDefinitionItem);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ReprocessDefinitionDetailMapper(this.GetReprocessDefinition(reprocessDefinitionItem.ReprocessDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<ReprocessDefinitionInfo> GetReprocessDefinitionsInfo(ReprocessDefinitionInfoFilter filter)
        {
            var allReprocessDefinitions = this.GetCachedReprocessDefinitions();

            if (filter != null)
            {
                Func<ReprocessDefinition, bool> filterExpression = (reprocessDefinition) =>
                {
                    if (filter != null)
                    {
                        if (filter.ExcludedReprocessDefinitionIds != null && filter.ExcludedReprocessDefinitionIds.Contains(reprocessDefinition.ReprocessDefinitionId))
                            return false;

                        if (filter.Filters != null)
                        {
                            ReprocessDefinitionFilterContext context = new ReprocessDefinitionFilterContext() { ReprocessDefinition = reprocessDefinition };
                            foreach (IReprocessDefinitionFilter reprocessDefinitionFilter in filter.Filters)
                            {
                                if (!reprocessDefinitionFilter.IsMatched(context))
                                    return false;
                            }
                        }
                    }
                    return true;
                };
                return allReprocessDefinitions.FindAllRecords(filterExpression).MapRecords(ReprocessDefinitionMapper);
            }
            else
            {
                return allReprocessDefinitions.MapRecords(ReprocessDefinitionMapper);
            }
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IReprocessDefinitionDataManager _dataManager = ReprocessDataManagerFactory.GetDataManager<IReprocessDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreReprocessDefinitionUpdated(ref _updateHandle);
            }
        }

        private class ReprocessDefinitionLoggableEntity : VRLoggableEntityBase
        {
            public static ReprocessDefinitionLoggableEntity Instance = new ReprocessDefinitionLoggableEntity();

            private ReprocessDefinitionLoggableEntity()
            {

            }

            static ReprocessDefinitionManager s_reprocessDefinitionManager = new ReprocessDefinitionManager();

            public override string EntityUniqueName
            {
                get { return "Reprocess_ReprocessDefinition"; }
            }

            public override string ModuleName
            {
                get { return "Reprocess"; }
            }

            public override string EntityDisplayName
            {
                get { return "Reprocess Definition"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "Reprocess_ReprocessDefinition_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                ReprocessDefinition reprocessDefinition = context.Object.CastWithValidate<ReprocessDefinition>("context.Object");
                return reprocessDefinition.ReprocessDefinitionId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                ReprocessDefinition reprocessDefinition = context.Object.CastWithValidate<ReprocessDefinition>("context.Object");
                return s_reprocessDefinitionManager.GetReprocessDefinitionName(reprocessDefinition);
            }
        }

        #endregion


        #region Private Methods

        Dictionary<Guid, ReprocessDefinition> GetCachedReprocessDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetReprocessDefinitions",
               () =>
               {
                   IReprocessDefinitionDataManager dataManager = ReprocessDataManagerFactory.GetDataManager<IReprocessDefinitionDataManager>();
                   return dataManager.GetReprocessDefinition().ToDictionary(x => x.ReprocessDefinitionId, x => x);
               });
        }

        #endregion


        #region Mappers

        private ReprocessDefinitionDetail ReprocessDefinitionDetailMapper(ReprocessDefinition reprocessDefinition)
        {
            ReprocessDefinitionDetail reprocessDefinitionDetail = new ReprocessDefinitionDetail()
            {
                Entity = reprocessDefinition
            };
            return reprocessDefinitionDetail;
        }


        private ReprocessDefinitionInfo ReprocessDefinitionMapper(ReprocessDefinition reprocessDefinition)
        {
            if (reprocessDefinition == null)
                return null;

            return new ReprocessDefinitionInfo()
            {
                ReprocessDefinitionId = reprocessDefinition.ReprocessDefinitionId,
                Name = reprocessDefinition.Name,
                ForceUseTempStorage = reprocessDefinition.Settings != null ? reprocessDefinition.Settings.ForceUseTempStorage : false
            };
        }
        #endregion
    }
}
