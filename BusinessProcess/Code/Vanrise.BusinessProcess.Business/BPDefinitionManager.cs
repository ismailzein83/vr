﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPDefinitionManager : IBPDefinitionManager
    {
        #region Ctor/Properties

        static SecurityManager s_securityManager = new SecurityManager();
        VRDevProjectManager vrDevProjectManager = new VRDevProjectManager();

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<BPDefinitionDetail> GetFilteredBPDefinitions(Vanrise.Entities.DataRetrievalInput<BPDefinitionQuery> input, int? viewableByUserId = null)
        {
            var allBPDefinitions = GetCachedBPDefinitions();

            Func<BPDefinition, bool> filterExpression = (prod) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(prod.DevProjectId))
                    return false;

                if (input.Query.ShowOnlyVisibleInManagementScreen == true && prod.Configuration.NotVisibleInManagementScreen)
                    return false;

                if (!string.IsNullOrEmpty(input.Query.Title) && !prod.Title.ToLower().Contains(input.Query.Title.ToLower()))
                    return false;
                if (input.Query.DevProjectIds != null && (!prod.DevProjectId.HasValue || !input.Query.DevProjectIds.Contains(prod.DevProjectId.Value)))
                    return false;
                if (viewableByUserId.HasValue && !DoesUserHaveViewAccess((int)viewableByUserId, prod))
                    return false;

                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(BPDefinitionLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allBPDefinitions.ToBigResult(input, filterExpression, (bpDefinition) => { return BPDefinitionDetailMapper(bpDefinition, viewableByUserId); }));
        }

        public IEnumerable<BPDefinitionInfo> GetBPDefinitionsInfo(BPDefinitionInfoFilter filter)
        {
            var bpDefinitions = GetCachedBPDefinitions();

            Func<BPDefinition, bool> filterExpression = (prod) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(prod.DevProjectId))
                    return false;

                if (!DoesUserHaveViewAccess(SecurityContext.Current.GetLoggedInUserId(), prod))
                    return false;

                if (filter != null && filter.Filters != null && !CheckIfFilterIsMatch(prod, filter.Filters))
                    return false;

                return true;
            };

            return bpDefinitions.FindAllRecords(filterExpression).MapRecords(BPDefinitionInfoMapper);
        }

        public InsertOperationOutput<BPDefinitionDetail> AddBPDefinition(BPDefinition bpDefinition)
        {
            var insertBPDefinitionOperationOutput = new InsertBPDefinitionOperationOutput();
            insertBPDefinitionOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertBPDefinitionOperationOutput.InsertedObject = null;

            List<string> errorMessages = null;
            if (bpDefinition.VRWorkflowId.HasValue)
                errorMessages = CompileProcessTitle(bpDefinition);

            if (errorMessages == null)
            {
                IBPDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionDataManager>();

                bpDefinition.BPDefinitionID = Guid.NewGuid();
                bpDefinition.Name = string.Concat("VRWorkflowInputArgument_", bpDefinition.BPDefinitionID.ToString("N"));

                if (dataManager.InsertBPDefinition(bpDefinition))
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    VRActionLogger.Current.TrackAndLogObjectAdded(BPDefinitionLoggableEntity.Instance, bpDefinition);
                    insertBPDefinitionOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    insertBPDefinitionOperationOutput.InsertedObject = BPDefinitionDetailMapper(bpDefinition);
                }
                else
                {
                    insertBPDefinitionOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                }
            }
            else
                insertBPDefinitionOperationOutput.ValidationMessages = errorMessages;

            return insertBPDefinitionOperationOutput;
        }

        public UpdateOperationOutput<BPDefinitionDetail> UpdateBPDefinition(BPDefinition bPDefinition)
        {
            var updateBPDefinitionOperationOutput = new UpdateBPDefinitionOperationOutput();
            updateBPDefinitionOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateBPDefinitionOperationOutput.UpdatedObject = null;

            List<string> errorMessages = null;
            if (bPDefinition.VRWorkflowId.HasValue)
                errorMessages = CompileProcessTitle(bPDefinition);

            if (errorMessages == null)
            {
                IBPDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionDataManager>();
                bool updateActionSucc = dataManager.UpdateBPDefinition(bPDefinition);

                if (updateActionSucc)
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    VRActionLogger.Current.TrackAndLogObjectUpdated(BPDefinitionLoggableEntity.Instance, bPDefinition);
                    updateBPDefinitionOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    updateBPDefinitionOperationOutput.UpdatedObject = BPDefinitionDetailMapper(bPDefinition);
                }
                else
                {
                    updateBPDefinitionOperationOutput.Result = UpdateOperationResult.SameExists;
                }
            }
            else
                updateBPDefinitionOperationOutput.ValidationMessages = errorMessages;

            return updateBPDefinitionOperationOutput;
        }

        public IEnumerable<BPDefinition> GetBPDefinitions()
        {
            var cachedDefinitions = GetCachedBPDefinitions();
            if (cachedDefinitions != null)
                return cachedDefinitions.Values;
            else
                return null;
        }

        public IEnumerable<BPDefinition> GetBPDefinitions(BPDefinitionInfoFilter filter)
        {
            var cachedDefinitions = GetCachedBPDefinitions();
            Func<BPDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (prod) =>
                {
                    if (filter != null && filter.Filters != null && !CheckIfFilterIsMatch(prod, filter.Filters))
                        return false;
                    return true;
                };
            }


            return cachedDefinitions.FindAllRecords(filterExpression);
        }

        public BPDefinition GetBPDefinition(Guid definitionId)
        {
            return GetCachedBPDefinitions().GetRecord(definitionId);
        }

        public BPDefinition GetDefinition(string processName)
        {
            return GetBPDefinitions().FirstOrDefault(itm => itm.Name == processName);
        }

        public string GetBPDefinitionName(BPDefinition bPDefinition)
        {
            return (bPDefinition != null) ? bPDefinition.Name : null;
        }

        public string GetDefinitionTitle(string processName)
        {
            var definition = GetDefinition(processName);
            return definition != null ? definition.Title : null;
        }

        public RequiredPermissionSettings GetViewInstanceRequiredPermissions(BPDefinition bpDefinition, BaseProcessInputArgument inputArg)
        {
            var getViewInstanceRequiredPermissionsContext = new BPDefinitionGetViewInstanceRequiredPermissionsContext
            {
                BPDefinition = bpDefinition,
                InputArg = inputArg
            };
            return GetBPDefinitionExtendedSettings(bpDefinition).GetViewInstanceRequiredPermissions(getViewInstanceRequiredPermissionsContext);
        }

        public BPDefinitionExtendedSettings GetBPDefinitionExtendedSettings(BPDefinition bPDefinition)
        {
            if (bPDefinition != null && bPDefinition.Configuration != null && bPDefinition.Configuration.ExtendedSettings != null)
                return bPDefinition.Configuration.ExtendedSettings;
            return new DefaultBPDefinitionExtendedSettings();
        }

        public Dictionary<Guid, BPDefinition> GetCachedBPDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBPDefinitions",
               () =>
               {
                   IBPDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionDataManager>();
                   IEnumerable<BPDefinition> accounts = dataManager.GetBPDefinitions();
                   return accounts.ToDictionary(cn => cn.BPDefinitionID, cn => cn);
               });
        }

        public Dictionary<Guid, Type> GetInputArgumentTypeByDefinitionId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<BPDefinitionWorkflowCacheManager>().GetOrCreateObject("GetInputArgumentTypeByDefinitionId",
               () =>
               {
                   Dictionary<Guid, Type> inputArgumentTypeByDefinitionId = new Dictionary<Guid, Type>();

                   IEnumerable<BPDefinition> bpDefinitions = this.GetBPDefinitions();
                   foreach (var bpDefinition in bpDefinitions)
                   {
                       if (!bpDefinition.VRWorkflowId.HasValue)
                           continue;

                       List<string> errorMessages;
                       inputArgumentTypeByDefinitionId.Add(bpDefinition.BPDefinitionID, Helper.GetInputArgumentType(bpDefinition, true, out errorMessages));
                   }

                   return inputArgumentTypeByDefinitionId;
               });
        }

        List<string> CompileProcessTitle(BPDefinition bPDefinition)
        {
            List<string> errorMessages;
            Helper.GetInputArgumentType(bPDefinition, false, out errorMessages);
            return errorMessages;
        }

        public BPDefinitionTitleCompilationOutput TryCompileProcessTitle(BPDefinition bPDefinition)
        {
            List<string> errorMessages = CompileProcessTitle(bPDefinition);
            return new BPDefinitionTitleCompilationOutput()
            {
                ErrorMessages = errorMessages,
                IsSucceeded = errorMessages == null || errorMessages.Count == 0
            };
        }

        #region Security

        public bool DoesUserHaveViewAccessInManagement(int userId)
        {
            var allPB = GetCachedBPDefinitions().Select(x => x.Value).Where(x => !x.Configuration.NotVisibleInManagementScreen);
            foreach (var bp in allPB)
            {
                if (DoesUserHaveViewAccess(userId, bp))
                    return true;
            }
            return false;
        }

        public bool DoesUserHaveViewAccessInSchedule(int userId)
        {
            var allPB = GetCachedBPDefinitions().Select(x => x.Value).Where(x => x.Configuration.ScheduledExecEditor != null);
            foreach (var bp in allPB)
            {
                if (DoesUserHaveViewAccess(userId, bp))
                    return true;
            }
            return false;
        }

        public bool DoesUserHaveScheduleTaskAccess(int userId)
        {
            var allPB = GetCachedBPDefinitions().Select(x => x.Value).Where(x => x.Configuration.ScheduledExecEditor != null);
            foreach (var bp in allPB)
            {
                if (DoesUserHaveScheduleTaskAccess(userId, bp))
                    return true;
            }
            return false;
        }

        public bool DoesUserHaveViewAccess(string bPDefinitionName)
        {
            var bPDefinition = GetDefinition(bPDefinitionName);
            return DoesUserHaveViewAccess(SecurityContext.Current.GetLoggedInUserId(), bPDefinition);

        }

        public bool DoesUserHaveViewAccess(int userId, BPDefinition bPDefinition)
        {
            var definitionContext = new BPDefinitionDoesUserHaveViewAccessContext { UserId = userId, BPDefinition = bPDefinition };
            return GetBPDefinitionExtendedSettings(bPDefinition).DoesUserHaveViewAccess(definitionContext);
        }

        public bool DoesUserHaveViewAccess(int userId, BPDefinition bPDefinition, BaseProcessInputArgument inputArg)
        {
            RequiredPermissionSettings viewInstanceRequiredPermissions = GetViewInstanceRequiredPermissions(bPDefinition, inputArg);
            return s_securityManager.IsAllowed(viewInstanceRequiredPermissions, userId);
        }

        public bool DoesUserHaveScheduleTaskAccess(int userId, Guid bPDefinitionId)
        {
            var bPDefinition = GetBPDefinition(bPDefinitionId);
            return DoesUserHaveScheduleTaskAccess(userId, bPDefinition);
        }

        public bool DoesUserHaveScheduleTaskAccess(int userId, BPDefinition bPDefinition)
        {
            var definitionContext = new BPDefinitionDoesUserHaveScheduleTaskContext { UserId = userId, BPDefinition = bPDefinition };
            return GetBPDefinitionExtendedSettings(bPDefinition).DoesUserHaveScheduleTaskAccess(definitionContext);

        }

        public bool DoesUserHaveStartNewInstanceAccess(int userId, BPDefinition bPDefinition)
        {
            var definitionContext = new BPDefinitionDoesUserHaveStartAccessContext { UserId = userId, BPDefinition = bPDefinition };
            return GetBPDefinitionExtendedSettings(bPDefinition).DoesUserHaveStartAccess(definitionContext);

        }

        public bool DoesUserHaveStartNewInstanceAccess(int userId, CreateProcessInput processInput)
        {
            return DoesUserHaveStartNewInstanceAccess(userId, processInput.InputArguments);

        }

        public bool DoesUserHaveStartNewInstanceAccess(int userId, BaseProcessInputArgument inputArg)
        {
            var bPDefinition = GetDefinition(inputArg.ProcessName);

            var context = new BPDefinitionDoesUserHaveStartSpecificInstanceAccessContext
            {
                DefinitionContext = new BPDefinitionDoesUserHaveStartAccessContext
                {
                    UserId = userId,
                    BPDefinition = bPDefinition
                },
                InputArg = inputArg

            };
            return GetBPDefinitionExtendedSettings(bPDefinition).DoesUserHaveStartSpecificInstanceAccess(context);

        }

        public bool DoesUserHaveScheduleSpecificTaskAccess(int userId, BaseProcessInputArgument inputArgument)
        {
            var bPDefinition = GetDefinition(inputArgument.ProcessName);

            var context = new BPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext
            {
                DefinitionContext = new BPDefinitionDoesUserHaveScheduleTaskContext
                {
                    UserId = userId,
                    BPDefinition = bPDefinition
                },
                InputArg = inputArgument

            };
            return GetBPDefinitionExtendedSettings(bPDefinition).DoesUserHaveScheduleSpecificTaskAccess(context);

        }

        #endregion

        #endregion

        #region Private Methods

        private bool DoesUserHaveBPPermission(int userId, RequiredPermissionSettings permission)
        {
            SecurityManager secManager = new SecurityManager();
            return secManager.IsAllowed(permission, userId);
        }

        private bool CheckIfFilterIsMatch(BPDefinition BPDefinition, List<IBPDefinitionFilter> filters)
        {
            var context = new BPDefinitionFilterContext { BPDefinitionId = BPDefinition.BPDefinitionID };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        #endregion

        #region Internal/Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionDataManager>();
            object _lastReceivedDataInfo;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPDefinitionsUpdated(ref _lastReceivedDataInfo);
            }
        }

        internal class BPDefinitionWorkflowCacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _bpDefinitionCacheLastCheck;
            DateTime? _vrWorkflowCacheLastCheck;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref _bpDefinitionCacheLastCheck)
                       | Vanrise.Caching.CacheManagerFactory.GetCacheManager<VRWorkflowManager.CacheManager>().IsCacheExpired(ref _vrWorkflowCacheLastCheck);
            }
        }

        private class BPDefinitionLoggableEntity : VRLoggableEntityBase
        {
            public static BPDefinitionLoggableEntity Instance = new BPDefinitionLoggableEntity();

            private BPDefinitionLoggableEntity()
            {

            }

            static BPDefinitionManager s_bPDefinitionManager = new BPDefinitionManager();

            public override string EntityUniqueName
            {
                get { return "BusinessProcess_BP_BPDefinition"; }
            }

            public override string ModuleName
            {
                get { return "Business Process"; }
            }

            public override string EntityDisplayName
            {
                get { return "BPDefinition"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "BusinessProcess_BP_BPDefinition_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                BPDefinition bPDefinition = context.Object.CastWithValidate<BPDefinition>("context.Object");
                return bPDefinition.BPDefinitionID;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                BPDefinition bPDefinition = context.Object.CastWithValidate<BPDefinition>("context.Object");
                return s_bPDefinitionManager.GetBPDefinitionName(bPDefinition);
            }
        }

        #endregion

        #region Mapper

        private BPDefinitionDetail BPDefinitionDetailMapper(BPDefinition bpDefinition, int? userID = null)
        {
            if (bpDefinition == null)
                return null;

            BPDefinitionDetail bpDefinitionDetail = new BPDefinitionDetail()
            {
                Entity = bpDefinition,
            };
            bpDefinitionDetail.ScheduleTaskAccess = true;
            bpDefinitionDetail.StartNewInstanceAccess = true;
            if (userID.HasValue)
            {

                bpDefinitionDetail.ScheduleTaskAccess = DoesUserHaveScheduleTaskAccess(userID.Value, bpDefinition); // bpDefinition.Configuration.Security !=null ? DoesUserHaveBPPermission((int)userID, bpDefinition.Configuration.Security.ScheduleTask) : true;
                bpDefinitionDetail.StartNewInstanceAccess = DoesUserHaveStartNewInstanceAccess(userID.Value, bpDefinition);//bpDefinition.Configuration.Security !=null ?  DoesUserHaveBPPermission((int)userID, bpDefinition.Configuration.Security.StartNewInstance) : true;

            }
            if (bpDefinition.DevProjectId.HasValue)
            {
                bpDefinitionDetail.DevProjectName = vrDevProjectManager.GetVRDevProjectName(bpDefinition.DevProjectId.Value);
            }
            return bpDefinitionDetail;
        }

        private BPDefinitionInfo BPDefinitionInfoMapper(BPDefinition bpDefinition)
        {
            if (bpDefinition == null)
                return null;

            return new BPDefinitionInfo()
            {
                BPDefinitionID = bpDefinition.BPDefinitionID,
                Name = vrDevProjectManager.ConcatenateTitleAndDevProjectName(bpDefinition.DevProjectId, bpDefinition.Title),
            };
        }

        #endregion
    }
}