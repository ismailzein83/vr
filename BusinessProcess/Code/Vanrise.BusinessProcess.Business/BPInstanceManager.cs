﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPInstanceManager
    {
        static BPDefinitionManager s_bpDefinitionManager = new BPDefinitionManager();

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<BPInstanceDetail> GetFilteredBPInstances(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new BPInstanceRequestHandler());
        }

        public List<BPInstance> GetAllFilteredBPInstances(BPInstanceQuery query)
        {
            var requiredPermissionSetManager = new RequiredPermissionSetManager();

            List<int> grantedPermissionSetIds;
            bool isUserGrantedAllModulePermissionSets = requiredPermissionSetManager.IsCurrentUserGrantedAllModulePermissionSets(BPInstance.REQUIREDPERMISSIONSET_MODULENAME, out grantedPermissionSetIds);

            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            List<BPInstance> bpInstances = bpInstanceDataManager.GetFilteredBPInstances(query, grantedPermissionSetIds, false);
            List<BPInstance> bpInstancesArchived = bpInstanceDataManager.GetFilteredBPInstances(query, grantedPermissionSetIds, true);
            if (bpInstancesArchived != null && bpInstancesArchived.Count > 0)
            {
                bpInstances.AddRange(bpInstancesArchived);
                bpInstances = bpInstances.OrderByDescending(bp => bp.ProcessInstanceID).Take(query.Top).ToList();
            }
            return bpInstances;
        }

        public BPInstance GetBPInstance(long bpInstanceId)
        {
            var bpInstance = GetBPInstance(bpInstanceId, false);
            if (bpInstance == null)
                bpInstance = GetBPInstance(bpInstanceId, true); //get from archive
            return bpInstance;
        }

        public BPInstance GetBPInstance(long bpInstanceId, bool getFromArchive)
        {
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            return bpInstanceDataManager.GetBPInstance(bpInstanceId, getFromArchive);
        }

        public string GetBPInstanceName(long bpInstanceId)
        {
            BPInstance bpInstance = GetBPInstance(bpInstanceId);
            return GetBPInstanceName(bpInstance);
        }

        public string GetBPInstanceName(BPInstance bpInstance)
        {
            return bpInstance != null ? bpInstance.Title : null;
        }

        public BPInstanceUpdateOutput GetUpdated(ref object lastUpdateHandle, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds, Guid? taskId)
        {
            List<int> grantedPermissionSetIds;
            bool isUserGrantedAllModulePermissionSets = new RequiredPermissionSetManager().IsCurrentUserGrantedAllModulePermissionSets(BPInstance.REQUIREDPERMISSIONSET_MODULENAME, out grantedPermissionSetIds);

            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();

            List<BPInstance> bpInstances;
            if (lastUpdateHandle == null) //first page
            {
                bpInstances = bpInstanceDataManager.GetFirstPage(out lastUpdateHandle, nbOfRows, definitionsId, parentId, entityIds, grantedPermissionSetIds, taskId);
                List<BPInstance> bpInstancesArchived = bpInstanceDataManager.GetFirstPageFromArchive(nbOfRows, definitionsId, parentId, entityIds, grantedPermissionSetIds, taskId);
                if (bpInstancesArchived != null && bpInstancesArchived.Count > 0)
                {
                    bpInstances.AddRange(bpInstancesArchived);
                    bpInstances = bpInstances.OrderByDescending(bp => bp.ProcessInstanceID).Take(nbOfRows).ToList();
                }
            }
            else
            {
                bpInstances = bpInstanceDataManager.GetUpdated(ref lastUpdateHandle, nbOfRows, definitionsId, parentId, entityIds, grantedPermissionSetIds, taskId);
            }

            List<BPInstanceDetail> bpInstanceDetails = new List<BPInstanceDetail>();
            foreach (BPInstance bpInstance in bpInstances)
                bpInstanceDetails.Add(BPInstanceDetailMapper(bpInstance));

            BPInstanceUpdateOutput bpInstanceUpdateOutput = new BPInstanceUpdateOutput();
            bpInstanceUpdateOutput.ListBPInstanceDetails = bpInstanceDetails;
            bpInstanceUpdateOutput.LastUpdateHandle = lastUpdateHandle;
            return bpInstanceUpdateOutput;
        }

        public List<BPInstanceDetail> GetBeforeId(BPInstanceBeforeIdInput input)
        {
            List<int> grantedPermissionSetIds;
            bool isUserGrantedAllModulePermissionSets = new RequiredPermissionSetManager().IsCurrentUserGrantedAllModulePermissionSets(BPInstance.REQUIREDPERMISSIONSET_MODULENAME, out grantedPermissionSetIds);

            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            List<BPInstance> bpInstances = bpInstanceDataManager.GetBeforeId(input, grantedPermissionSetIds, false);
            List<BPInstance> bpInstancesArchived = bpInstanceDataManager.GetBeforeId(input, grantedPermissionSetIds, true);
            if (bpInstancesArchived != null && bpInstancesArchived.Count > 0)
            {
                bpInstances.AddRange(bpInstancesArchived);
                bpInstances = bpInstances.OrderByDescending(bp => bp.ProcessInstanceID).Take(input.NbOfRows).ToList();
            }

            List<BPInstanceDetail> bpInstanceDetails = new List<BPInstanceDetail>();
            foreach (BPInstance bpInstance in bpInstances)
                bpInstanceDetails.Add(BPInstanceDetailMapper(bpInstance));

            return bpInstanceDetails;
        }

        public List<BPInstance> GetAfterId(long? processInstanceId, Guid bpDefinitionId)
        {
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            var bpInstances = bpInstanceDataManager.GetAfterId(processInstanceId, bpDefinitionId, false);
            var bpInstancesArchived = bpInstanceDataManager.GetAfterId(processInstanceId, bpDefinitionId, true);
            if (bpInstancesArchived != null && bpInstancesArchived.Count > 0)
                bpInstances.AddRange(bpInstancesArchived);
            return bpInstances;
        }

        public BPInstanceDefinitionDetail GetBPInstanceDefinitionDetail(Guid bpDefinitionId, long bpInstanceId)
        {
            var bpdefinition = s_bpDefinitionManager.GetBPDefinition(bpDefinitionId);
            return BPInstanceDefinitionDetailMapper(bpdefinition, bpInstanceId);
        }

        public CreateProcessOutput CreateNewProcess(CreateProcessInput createProcessInput)
        {
            return CreateNewProcess(createProcessInput, false);
        }

        public CreateProcessOutput CreateNewProcess(CreateProcessInput createProcessInput, bool isViewedFromUI)
        {
            if (createProcessInput == null)
                throw new ArgumentNullException("createProcessInput");
            if (createProcessInput.InputArguments == null)
                throw new ArgumentNullException("createProcessInput.InputArguments");
            if (createProcessInput.InputArguments.UserId == 0)
                throw new ArgumentException("createProcessInput.InputArguments.UserId");

            BPDefinitionManager bpDefinitionManager = new BPDefinitionManager();
            BPDefinition processDefinition = bpDefinitionManager.GetDefinition(createProcessInput.InputArguments.ProcessName);
            if (processDefinition == null)
                throw new Exception(String.Format("No Process Definition found match with input argument '{0}'", createProcessInput.InputArguments.GetType()));

            if (isViewedFromUI && processDefinition.Configuration != null && processDefinition.Configuration.ExtendedSettings != null &&
                processDefinition.Configuration.ExtendedSettings.StoreLastArgumentState)
            {
                new BPDefintionArgumentStateManager().InsertOrUpdateBPDefinitionArgumentState(new BPDefinitionArgumentState()
                {
                    BPDefinitionID = processDefinition.BPDefinitionID,
                    InputArgument = createProcessInput.InputArguments
                });
            }

            int? viewInstanceRequiredPermissionSetId = null;
            RequiredPermissionSettings viewInstanceRequiredPermissions = bpDefinitionManager.GetViewInstanceRequiredPermissions(processDefinition, createProcessInput.InputArguments);
            if (viewInstanceRequiredPermissions != null && viewInstanceRequiredPermissions.Entries != null && viewInstanceRequiredPermissions.Entries.Count > 0)
                viewInstanceRequiredPermissionSetId = new RequiredPermissionSetManager().GetRequiredPermissionSetId(BPInstance.REQUIREDPERMISSIONSET_MODULENAME, viewInstanceRequiredPermissions);

            string processTitle = createProcessInput.InputArguments.GetTitle();
            if (processTitle != null)
                processTitle = processTitle.Replace("#BPDefinitionTitle#", processDefinition.Title);

            BPInstanceToAdd bpInstanceToAdd = new Entities.BPInstanceToAdd()
            {
                Title = processTitle,
                ParentProcessID = createProcessInput.ParentProcessID,
                InitiatorUserId = createProcessInput.InputArguments.UserId,
                EntityId = createProcessInput.InputArguments.EntityId,
                DefinitionID = processDefinition.BPDefinitionID,
                Status = BPInstanceStatus.New,
                InputArgument = createProcessInput.InputArguments,
                ViewRequiredPermissionSetId = viewInstanceRequiredPermissionSetId,
                CompletionNotifier = createProcessInput.CompletionNotifier,
                TaskId = createProcessInput.TaskId
            };

            object handlerCustomData = null;
            if (processDefinition.Configuration != null && processDefinition.Configuration.BPInstanceInsertHandler != null)
            {
                var bpInstanceHandlerBeforeExecuteInsertContext = new BPInstanceHandlerBeforeExecuteInsertContext() { BPInstanceToAdd = bpInstanceToAdd, StartProcessOutput = createProcessInput.StartProcessOutput };
                processDefinition.Configuration.BPInstanceInsertHandler.ExecuteBeforeInsert(bpInstanceHandlerBeforeExecuteInsertContext);
                handlerCustomData = bpInstanceHandlerBeforeExecuteInsertContext.CustomData;
            }

            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            long processInstanceId = bpInstanceDataManager.InsertInstance(bpInstanceToAdd);

            IBPTrackingDataManager dataManagerTracking = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            dataManagerTracking.Insert(new BPTrackingMessage
            {
                ProcessInstanceId = processInstanceId,
                ParentProcessId = createProcessInput.ParentProcessID,
                TrackingMessage = String.Format("Process Created: {0}", processTitle),
                Severity = LogEntryType.Information,
                EventTime = DateTime.Now
            });

            BPInstance bpInstance = GetBPInstance(processInstanceId);
            if (bpInstance != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectCustomAction(new BPInstanceLoggableEntity(createProcessInput.InputArguments.GetDefinitionTitle()), "Start Process", false, bpInstance);

            if (processDefinition.Configuration != null && processDefinition.Configuration.BPInstanceInsertHandler != null)
            {
                var bpInstanceHandlerAfterExecuteInsertContext = new BPInstanceHandlerAfterExecuteInsertContext() { BPInstance = bpInstance, StartProcessOutput = createProcessInput.StartProcessOutput, CustomData = handlerCustomData };
                processDefinition.Configuration.BPInstanceInsertHandler.ExecuteAfterInsert(bpInstanceHandlerAfterExecuteInsertContext);
            }

            CreateProcessOutput output = new CreateProcessOutput
            {
                ProcessInstanceId = processInstanceId,
                Result = CreateProcessResult.Succeeded
            };
            return output;
        }

        public List<BPInstance> GetPendingInstances(Guid definitionId, IEnumerable<BPInstanceStatus> acceptableBPStatuses, BPInstanceAssignmentStatus assignmentStatus, int maxCounts, Guid serviceInstanceId)
        {
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            return bpInstanceDataManager.GetPendingInstances(definitionId, acceptableBPStatuses, assignmentStatus, maxCounts, serviceInstanceId);
        }

        public List<BPInstance> GetPendingInstancesInfo(IEnumerable<BPInstanceStatus> statuses)
        {
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            return bpInstanceDataManager.GetPendingInstancesInfo(statuses);
        }

        public void ArchiveInstances(List<BPInstanceStatus> completedStatuses, DateTime completedBefore, int nbOfInstances)
        {
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            bpInstanceDataManager.ArchiveInstances(completedStatuses, completedBefore, nbOfInstances);
        }

        public void UpdateInstanceStatus(long processInstanceId, BPInstanceStatus status, BPInstanceAssignmentStatus assignmentStatus, string message, bool clearServiceInstanceId, Guid? workflowInstanceId)
        {
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            bpInstanceDataManager.UpdateInstanceStatus(processInstanceId, status, assignmentStatus, message, clearServiceInstanceId, workflowInstanceId);
        }

        public void UpdateInstanceLastMessage(long processInstanceId, string message)
        {
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            bpInstanceDataManager.UpdateInstanceLastMessage(processInstanceId, message);
        }

        public void UpdateServiceInstancesAndAssignmentStatus(List<BPInstance> pendingInstancesToUpdate)
        {
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            bpInstanceDataManager.UpdateServiceInstancesAndAssignmentStatus(pendingInstancesToUpdate);
        }

        public void UpdateInstanceAssignmentStatus(long processInstanceId, BPInstanceAssignmentStatus assignmentStatus)
        {
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            bpInstanceDataManager.UpdateInstanceAssignmentStatus(processInstanceId, assignmentStatus);
        }

        public UpdateOperationOutput<object> CancelProcess(long bpInstanceId)
        {
            int cancelRequestByUserId = SecurityContext.Current.GetLoggedInUserId();
            List<BPInstanceStatus> allowedStatuses = BPInstanceStatusAttribute.GetNonClosedStatuses();

            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            bpInstanceDataManager.SetCancellationRequestUserId(bpInstanceId, allowedStatuses, cancelRequestByUserId);

            var bpInstance = GetBPInstance(bpInstanceId);
            bpInstance.ThrowIfNull("bpInstance", bpInstanceId);

            if (bpInstance.CancellationRequestByUserId == cancelRequestByUserId)
            {
                return new UpdateOperationOutput<object>
                {
                    Result = UpdateOperationResult.Succeeded
                };
            }
            else
            {
                return new UpdateOperationOutput<object>
                {
                    Result = UpdateOperationResult.Failed,
                    Message = String.Format("Process cannot be cancelled because it is in status '{0}'", Utilities.GetEnumDescription(bpInstance.Status)),
                    ShowExactMessage = true
                };
            }
        }

        public bool HasRunningInstances(Guid definitionId, List<string> entityIds)
        {
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            return bpInstanceDataManager.HasRunningInstances(definitionId, entityIds, BPInstanceStatusAttribute.GetNonClosedStatuses());
        }

        public bool DoesUserHaveCancelAccess(int userId, long bpInstanceId)
        {
            var bpInstance = GetBPInstance(bpInstanceId);
            bpInstance.ThrowIfNull("bpInstance", bpInstanceId);
            bpInstance.InputArgument.ThrowIfNull("bpInstance.InputArgument", bpInstanceId);
            var bpDefinition = s_bpDefinitionManager.GetBPDefinition(bpInstance.DefinitionID);
            bpDefinition.ThrowIfNull("bpDefinition", bpInstance.DefinitionID);
            return s_bpDefinitionManager.DoesUserHaveViewAccess(userId, bpDefinition, bpInstance.InputArgument)
                && s_bpDefinitionManager.DoesUserHaveStartNewInstanceAccess(userId, bpInstance.InputArgument);
        }

        public IEnumerable<BPInstanceInsertHandlerConfig> GetBPInstanceInsertHandlerConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<BPInstanceInsertHandlerConfig>(BPInstanceInsertHandlerConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Methods

        private IBPInstanceDataManager GetBPInstanceDataManager()
        {
            IBPInstanceDataManager bpInstanceDataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
            bpInstanceDataManager.InputArgumentTypeByDefinitionId = s_bpDefinitionManager.GetInputArgumentTypeByDefinitionId();
            return bpInstanceDataManager;
        }

        private List<Guid> GetFilteredTaskIdsByBPDefinitionId(Guid bpDefinitionID)
        {
            SchedulerTaskFilter filter = new SchedulerTaskFilter()
            {
                Filters = new List<ISchedulerTaskFilter>()
                {
                    new WFTaskBPDefinitionFilter(){
                        BPDefinitionId = bpDefinitionID
                    }
                },
                Status = SchedulerTaskFilterStatus.OnlyEnabled
            };
            List<Guid> taskIds = new SchedulerTaskManager().GetTasksInfo(filter, null).Select(x => x.TaskId).ToList();

            return taskIds;
        }

        #endregion

        #region Private Classes

        private class BPInstanceRequestHandler : BigDataRequestHandler<BPInstanceQuery, BPInstance, BPInstanceDetail>
        {
            static BPInstanceManager s_instanceManager = new BPInstanceManager();
            public override BPInstanceDetail EntityDetailMapper(BPInstance entity)
            {
                return s_instanceManager.BPInstanceDetailMapper(entity);
            }

            public override IEnumerable<BPInstance> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
            {
                var maxTop = new Vanrise.Common.Business.ConfigManager().GetMaxSearchRecordCount();
                if (input.Query.Top > maxTop)
                    throw new VRBusinessException(string.Format("Top record count cannot be greater than {0}", maxTop));
                return s_instanceManager.GetAllFilteredBPInstances(input.Query);
            }

            protected override ResultProcessingHandler<BPInstanceDetail> GetResultProcessingHandler(DataRetrievalInput<BPInstanceQuery> input, BigResult<BPInstanceDetail> bigResult)
            {
                return new ResultProcessingHandler<BPInstanceDetail>
                {
                    ExportExcelHandler = new BPInstanceExcelExportHandler(input.Query)
                };
            }
        }

        private class BPInstanceExcelExportHandler : ExcelExportHandler<BPInstanceDetail>
        {
            BPInstanceQuery _query;
            public BPInstanceExcelExportHandler(BPInstanceQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<BPInstanceDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Business Process",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Title", Width = 50 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Business Processes", Width = 50 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Last Message", Width = 50 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Status" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.ProcessInstanceID });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.CreatedTime });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Title });
                            row.Cells.Add(new ExportExcelCell { Value = record.DefinitionTitle });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.LastMessage });
                            row.Cells.Add(new ExportExcelCell { Value = record.StatusDescription });
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }

        public class BPInstanceLoggableEntity : VRLoggableEntityBase
        {
            string _entityDisplayName;

            static BPInstanceManager s_bpInstanceManager = new BPInstanceManager();

            public override string EntityUniqueName { get { return "VR_BusinessProcess_BPInstance"; } }

            public override string ModuleName { get { return "Business Process"; } }

            public override string EntityDisplayName { get { return this._entityDisplayName; } }

            public override string ViewHistoryItemClientActionName { get { return "VR_BusinessProcess_BPInstance_ViewHistoryItem"; } }

            public BPInstanceLoggableEntity(string entityDisplayName)
            {
                _entityDisplayName = entityDisplayName;
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                BPInstance bpInstance = context.Object.CastWithValidate<BPInstance>("context.Object");
                return bpInstance.ProcessInstanceID;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                BPInstance bpInstance = context.Object.CastWithValidate<BPInstance>("context.Object");
                return s_bpInstanceManager.GetBPInstanceName(bpInstance);
            }
        }

        public List<BPDefinitionSummaryDetail> GetBPDefinitionSummary()
        {
            Dictionary<Guid, BPDefinitionSummaryDetail> bpDefinitionSummaryDetails = new Dictionary<Guid, BPDefinitionSummaryDetail>();
            IBPInstanceDataManager bpInstanceDataManager = GetBPInstanceDataManager();
            var bpDefinitionSummaries = bpInstanceDataManager.GetBPDefinitionSummary(BPInstanceStatusAttribute.GetNonClosedStatuses());
            foreach (var bpSummary in bpDefinitionSummaries)
            {
                BPDefinitionSummaryDetail bpDefinitionSummaryDetail = new BPDefinitionSummaryDetail()
                {
                    BPDefinitionID = bpSummary.BPDefinitionID,
                    RunningProcessNumber = bpSummary.RunningProcessNumber,
                    PendingInstanceTime = bpSummary.PendingInstanceTime
                };
                var taskIds = GetFilteredTaskIdsByBPDefinitionId(bpSummary.BPDefinitionID);
                List<SchedulerTaskState> schedulerTaskStates = new SchedulerTaskStateManager().GetSchedulerTaskStateByTaskIds(taskIds);
                if (schedulerTaskStates != null && schedulerTaskStates.Count > 0)
                    bpDefinitionSummaryDetail.NextInstanceTime = schedulerTaskStates.Select(x => x.NextRunTime).Min();
                bpDefinitionSummaryDetails.Add(bpDefinitionSummaryDetail.BPDefinitionID, bpDefinitionSummaryDetail);
            }

            var bpDefinitions = new BPDefinitionManager().GetCachedBPDefinitions();
            foreach (var bp in bpDefinitions.Values)
            {
                if (bpDefinitionSummaryDetails.GetRecord(bp.BPDefinitionID) == null)
                {
                    var taskIds = GetFilteredTaskIdsByBPDefinitionId(bp.BPDefinitionID);
                    List<SchedulerTaskState> schedulerTaskStates = new SchedulerTaskStateManager().GetSchedulerTaskStateByTaskIds(taskIds);
                    if (schedulerTaskStates != null && schedulerTaskStates.Count > 0)
                    {
                        BPDefinitionSummaryDetail bpDefinitionSummaryDetail = new BPDefinitionSummaryDetail()
                        {
                            BPDefinitionID = bp.BPDefinitionID,
                            NextInstanceTime = schedulerTaskStates.Select(x => x.NextRunTime).Min()
                        };
                        if (bpDefinitionSummaryDetail.NextInstanceTime.HasValue)
                            bpDefinitionSummaryDetails.Add(bpDefinitionSummaryDetail.BPDefinitionID, bpDefinitionSummaryDetail);
                    }
                }
            }
            return bpDefinitionSummaryDetails.Values.ToList();
        }

        #endregion

        #region Mappers

        private BPInstanceDetail BPInstanceDetailMapper(BPInstance bpInstance)
        {
            if (bpInstance == null)
                return null;
            string bpDefinitionTitle = null;
            string userName = new UserManager().GetUserName(bpInstance.InitiatorUserId);
            var bpDefinition = new BPDefinitionManager().GetBPDefinition(bpInstance.DefinitionID);
            if (bpDefinition != null)
                bpDefinitionTitle = bpDefinition.Title;
            return new BPInstanceDetail()
            {
                Entity = bpInstance,
                DefinitionTitle = bpDefinitionTitle,
                UserName = userName
            };
        }

        private BPInstanceDefinitionDetail BPInstanceDefinitionDetailMapper(BPDefinition bpDefinition, long bpInstanceId)
        {
            BPInstanceDefinitionDetail detail = new BPInstanceDefinitionDetail();
            detail.Entity = bpDefinition;
            var extendedSettings = s_bpDefinitionManager.GetBPDefinitionExtendedSettings(bpDefinition);
            detail.AllowCancel = extendedSettings.CanCancelBPInstance(null) && DoesUserHaveCancelAccess(SecurityContext.Current.GetLoggedInUserId(), bpInstanceId);
            return detail;
        }

        #endregion
    }
}