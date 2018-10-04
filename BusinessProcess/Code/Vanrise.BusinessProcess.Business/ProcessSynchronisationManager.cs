using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class ProcessSynchronisationManager
    {
        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<ProcessSynchronisationDetail> GetFilteredProcessesSynchronisations(Vanrise.Entities.DataRetrievalInput<ProcessSynchronisationQuery> input)
        {
            var allProcessSynchronisations = GetCachedProcessSynchronisations();

            Func<ProcessSynchronisation, bool> filterExpression = (processSynchronisation) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Name) && !processSynchronisation.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.Statuses != null)
                {
                    if (!input.Query.Statuses.Contains(ProcessSynchronisationStatus.Disabled) && !processSynchronisation.IsEnabled)
                        return false;

                    if (!input.Query.Statuses.Contains(ProcessSynchronisationStatus.Enabled) && processSynchronisation.IsEnabled)
                        return false;
                }

                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(ProcessSynchronisationLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allProcessSynchronisations.ToBigResult(input, filterExpression, (processSynchronisation) => { return ProcessSynchronisationDetailMapper(processSynchronisation); }));
        }

        public InsertOperationOutput<ProcessSynchronisationDetail> AddProcessSynchronisation(ProcessSynchronisationToAdd processSynchronisationToAdd)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ProcessSynchronisationDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();

            IProcessSynchronisationDataManager dataManager = BPDataManagerFactory.GetDataManager<IProcessSynchronisationDataManager>();
            Guid processSynchronisationId = Guid.NewGuid();
            if (dataManager.InsertProcessSynchronisation(processSynchronisationId, processSynchronisationToAdd, loggedInUserId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                ProcessSynchronisation insertedProcessSynchronisation = GetProcessSynchronisation(processSynchronisationId);
                VRActionLogger.Current.TrackAndLogObjectAdded(ProcessSynchronisationLoggableEntity.Instance, insertedProcessSynchronisation);
                insertOperationOutput.InsertedObject = ProcessSynchronisationDetailMapper(insertedProcessSynchronisation);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<ProcessSynchronisationDetail> UpdateProcessSynchronisation(ProcessSynchronisationToUpdate processSynchronisationToUpdate)
        {
            UpdateOperationOutput<ProcessSynchronisationDetail> updateOperationOutput = new UpdateOperationOutput<ProcessSynchronisationDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();

            IProcessSynchronisationDataManager dataManager = BPDataManagerFactory.GetDataManager<IProcessSynchronisationDataManager>();
            bool updateActionSucc = dataManager.UpdateProcessSynchronisation(processSynchronisationToUpdate, loggedInUserId);

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                ProcessSynchronisation updatedProcessSynchronisation = GetProcessSynchronisation(processSynchronisationToUpdate.ProcessSynchronisationId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(ProcessSynchronisationLoggableEntity.Instance, updatedProcessSynchronisation);
                updateOperationOutput.UpdatedObject = ProcessSynchronisationDetailMapper(updatedProcessSynchronisation);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<ProcessSynchronisation> GetProcessSynchronisations()
        {
            var cachedProcessSynchronisations = GetCachedProcessSynchronisations();
            if (cachedProcessSynchronisations != null)
                return cachedProcessSynchronisations.Values;
            else
                return null;
        }

        public ProcessSynchronisation GetProcessSynchronisation(Guid processSynchronisationId)
        {
            var cachedProcessSynchronisations = GetCachedProcessSynchronisations();
            if (cachedProcessSynchronisations != null)
                return cachedProcessSynchronisations.GetRecord(processSynchronisationId);
            else
                return null;
        }

        public StructuredProcessSynchronisation GetStructuredProcessSynchronisation(Guid bpDefinitionId)
        {
            var cachedStructuredProcessSynchronisations = GetCachedStructuredProcessSynchronisationsByBPDefinition();
            if (cachedStructuredProcessSynchronisations != null)
                return cachedStructuredProcessSynchronisations.GetRecord(bpDefinitionId);
            else
                return null;
        }

        public object EnableProcessSynchronisation(Guid processSynchronisationId)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ProcessSynchronisationDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IProcessSynchronisationDataManager dataManager = BPDataManagerFactory.GetDataManager<IProcessSynchronisationDataManager>();

            var lastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();

            if (dataManager.EnableProcessSynchronisation(processSynchronisationId, lastModifiedBy))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var processSynchronisation = GetProcessSynchronisation(processSynchronisationId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(ProcessSynchronisationLoggableEntity.Instance, processSynchronisation);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ProcessSynchronisationDetailMapper(processSynchronisation);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public object DisableProcessSynchronisation(Guid processSynchronisationId)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ProcessSynchronisationDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IProcessSynchronisationDataManager dataManager = BPDataManagerFactory.GetDataManager<IProcessSynchronisationDataManager>();

            var lastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();

            if (dataManager.DisableProcessSynchronisation(processSynchronisationId, lastModifiedBy))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var processSynchronisation = GetProcessSynchronisation(processSynchronisationId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(ProcessSynchronisationLoggableEntity.Instance, processSynchronisation);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ProcessSynchronisationDetailMapper(processSynchronisation);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        #endregion

        #region Private Methods
        Dictionary<Guid, ProcessSynchronisation> GetCachedProcessSynchronisations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetProcessSynchronisations",
               () =>
               {
                   IProcessSynchronisationDataManager dataManager = BPDataManagerFactory.GetDataManager<IProcessSynchronisationDataManager>();
                   IEnumerable<ProcessSynchronisation> processSynchronisations = dataManager.GetProcessSynchronisations();
                   return processSynchronisations.ToDictionary(cn => cn.ProcessSynchronisationId, cn => cn);
               });
        }

        Dictionary<Guid, StructuredProcessSynchronisation> GetCachedStructuredProcessSynchronisationsByBPDefinition()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetStructuredProcessSynchronisationsByBPDefinition",
               () =>
               {
                   Dictionary<Guid, StructuredProcessSynchronisation> result = new Dictionary<Guid, StructuredProcessSynchronisation>();
                   Dictionary<Guid, ProcessSynchronisation> cachedProcessSynchronisations = GetCachedProcessSynchronisations();
                   if (cachedProcessSynchronisations != null)
                   {
                       foreach (var processSynchronisationKvp in cachedProcessSynchronisations)
                       {
                           ProcessSynchronisation currentProcessSynchronisation = processSynchronisationKvp.Value;
                           ProcessSynchronisationSettings settings = currentProcessSynchronisation.Settings;
                           settings.ThrowIfNull("settings", processSynchronisationKvp.Key);
                           settings.FirstProcessSynchronisationGroup.ThrowIfNull("settings.FirstProcessSynchronisationGroup", processSynchronisationKvp.Key);
                           settings.SecondProcessSynchronisationGroup.ThrowIfNull("settings.SecondProcessSynchronisationGroup", processSynchronisationKvp.Key);

                           var firstProcessSynchronisationGroup = settings.FirstProcessSynchronisationGroup;
                           var secondProcessSynchronisationGroup = settings.SecondProcessSynchronisationGroup;

                           FillLinkedProcessSynchronisationItems(result, firstProcessSynchronisationGroup, secondProcessSynchronisationGroup);
                           FillLinkedProcessSynchronisationItems(result, secondProcessSynchronisationGroup, firstProcessSynchronisationGroup);
                       }
                   }

                   return result;
               });
        }

        void FillLinkedProcessSynchronisationItems(Dictionary<Guid, StructuredProcessSynchronisation> result, ProcessSynchronisationGroup mainGroup, ProcessSynchronisationGroup dependantGroup)
        {
            if (mainGroup.BPSynchronisationItems != null)
            {
                foreach (var mainGroupBPSynchronisationItem in mainGroup.BPSynchronisationItems)
                {
                    StructuredProcessSynchronisation structuredProcessSynchronisation = result.GetOrCreateItem(mainGroupBPSynchronisationItem.BPDefinitionId);
                    if (mainGroupBPSynchronisationItem.TaskIds != null && mainGroupBPSynchronisationItem.TaskIds.Count > 0)
                    {
                        foreach (var taskId in mainGroupBPSynchronisationItem.TaskIds)
                        {
                            var linkedProcessSynchronisationItems = structuredProcessSynchronisation.LinkedProcessSynchronisationItemsByTaskId.GetOrCreateItem(taskId);
                            UpdateLinkedProcessSynchronisationItems(linkedProcessSynchronisationItems, dependantGroup);
                        }
                    }
                    else
                    {
                        UpdateLinkedProcessSynchronisationItems(structuredProcessSynchronisation.LinkedProcessSynchronisationItems, dependantGroup);
                    }
                }
            }
        }

        void UpdateLinkedProcessSynchronisationItems(LinkedProcessSynchronisationItems linkedProcessSynchronisationItems, ProcessSynchronisationGroup dependantProcessSynchronisationGroup)
        {
            if (dependantProcessSynchronisationGroup.BPSynchronisationItems != null && dependantProcessSynchronisationGroup.BPSynchronisationItems.Count > 0)
            {
                foreach (var dependantGroupBPSynchronisationItem in dependantProcessSynchronisationGroup.BPSynchronisationItems)
                {
                    if (dependantGroupBPSynchronisationItem.TaskIds != null && dependantGroupBPSynchronisationItem.TaskIds.Count > 0)
                    {
                        if (linkedProcessSynchronisationItems.TaskIds == null)
                            linkedProcessSynchronisationItems.TaskIds = new HashSet<Guid>();

                        foreach (var taskId in dependantGroupBPSynchronisationItem.TaskIds)
                        {
                            linkedProcessSynchronisationItems.TaskIds.Add(taskId);
                        }
                    }
                    else
                    {
                        if (linkedProcessSynchronisationItems.BPDefinitionIds == null)
                            linkedProcessSynchronisationItems.BPDefinitionIds = new HashSet<Guid>();

                        linkedProcessSynchronisationItems.BPDefinitionIds.Add(dependantGroupBPSynchronisationItem.BPDefinitionId);
                    }
                }
            }

            if (dependantProcessSynchronisationGroup.ExecutionFlowSynchronisationItems != null && dependantProcessSynchronisationGroup.ExecutionFlowSynchronisationItems.Count > 0)
            {
                if (linkedProcessSynchronisationItems.ExecutionFlowDefinitionIds == null)
                    linkedProcessSynchronisationItems.ExecutionFlowDefinitionIds = new HashSet<Guid>();

                foreach (var dependantGroupExecutionFlowSynchronisationItem in dependantProcessSynchronisationGroup.ExecutionFlowSynchronisationItems)
                {
                    linkedProcessSynchronisationItems.ExecutionFlowDefinitionIds.Add(dependantGroupExecutionFlowSynchronisationItem.ExecutionFlowDefinitionId);
                }
            }
        }
        #endregion

        #region Internal/Private Classes
        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IProcessSynchronisationDataManager dataManager = BPDataManagerFactory.GetDataManager<IProcessSynchronisationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreProcessSynchronisationsUpdated(ref _updateHandle);
            }
        }

        private class ProcessSynchronisationLoggableEntity : VRLoggableEntityBase
        {
            public static ProcessSynchronisationLoggableEntity Instance = new ProcessSynchronisationLoggableEntity();

            private ProcessSynchronisationLoggableEntity()
            {

            }

            public override string EntityUniqueName
            {
                get { return "BusinessProcess_ProcessSynchronisation"; }
            }

            public override string ModuleName
            {
                get { return "Process Synchronisation"; }
            }

            public override string EntityDisplayName
            {
                get { return "ProcessSynchronisation"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "BusinessProcess_BP_ProcessSynchronisation_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                ProcessSynchronisation processSynchronisation = context.Object.CastWithValidate<ProcessSynchronisation>("context.Object");
                return processSynchronisation.ProcessSynchronisationId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                ProcessSynchronisation processSynchronisation = context.Object.CastWithValidate<ProcessSynchronisation>("context.Object");
                return processSynchronisation.Name;
            }
        }
        #endregion

        #region Mapper
        private ProcessSynchronisationDetail ProcessSynchronisationDetailMapper(ProcessSynchronisation processSynchronisation)
        {
            if (processSynchronisation == null)
                return null;

            return new ProcessSynchronisationDetail()
            {
                ProcessSynchronisationId = processSynchronisation.ProcessSynchronisationId,
                Name = processSynchronisation.Name,
                IsEnabled = processSynchronisation.IsEnabled
            };
        }
        #endregion
    }
}