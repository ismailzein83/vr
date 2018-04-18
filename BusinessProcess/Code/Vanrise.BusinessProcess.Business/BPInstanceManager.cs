using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using System.Linq;

namespace Vanrise.BusinessProcess.Business
{
    public class BPInstanceManager
    {
        static BPDefinitionManager s_definitionManager = new BPDefinitionManager();
        static IBPInstanceDataManager s_dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
        #region public methods

        public BPInstance GetBPInstance(long bpInstanceId)
        {
            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
            return dataManager.GetBPInstance(bpInstanceId);
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
        public Vanrise.Entities.IDataRetrievalResult<BPInstanceDetail> GetFilteredBPInstances(Vanrise.Entities.DataRetrievalInput<BPInstanceQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new BPInstanceRequestHandler());
        }

        public List<BPInstance> GetAllFilteredBPInstances(BPInstanceQuery query)
        {
            var requiredPermissionSetManager = new RequiredPermissionSetManager();
            List<int> grantedPermissionSetIds;
            bool isUserGrantedAllModulePermissionSets = requiredPermissionSetManager.IsCurrentUserGrantedAllModulePermissionSets(BPInstance.REQUIREDPERMISSIONSET_MODULENAME, out grantedPermissionSetIds);
            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
            return dataManager.GetAllBPInstances(query, grantedPermissionSetIds);
        }

        public BPInstanceUpdateOutput GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, List<Guid> definitionsId, int parentId, List<string> entityIds)
        {
            BPInstanceUpdateOutput bpInstanceUpdateOutput = new BPInstanceUpdateOutput();

            var requiredPermissionSetManager = new RequiredPermissionSetManager();
            List<int> grantedPermissionSetIds;
            bool isUserGrantedAllModulePermissionSets = requiredPermissionSetManager.IsCurrentUserGrantedAllModulePermissionSets(BPInstance.REQUIREDPERMISSIONSET_MODULENAME, out grantedPermissionSetIds);
            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();

            List<BPInstance> bpInstances = dataManager.GetUpdated(ref maxTimeStamp, nbOfRows, definitionsId, parentId, entityIds, grantedPermissionSetIds);
            List<BPInstanceDetail> bpInstanceDetails = new List<BPInstanceDetail>();
            foreach (BPInstance bpInstance in bpInstances)
            {
                bpInstanceDetails.Add(BPInstanceDetailMapper(bpInstance));
            }

            bpInstanceUpdateOutput.ListBPInstanceDetails = bpInstanceDetails;
            bpInstanceUpdateOutput.MaxTimeStamp = maxTimeStamp;
            return bpInstanceUpdateOutput;
        }

        public List<BPInstanceDetail> GetBeforeId(BPInstanceBeforeIdInput input)
        {
            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
            var requiredPermissionSetManager = new RequiredPermissionSetManager();
            List<int> grantedPermissionSetIds;
            bool isUserGrantedAllModulePermissionSets = requiredPermissionSetManager.IsCurrentUserGrantedAllModulePermissionSets(BPInstance.REQUIREDPERMISSIONSET_MODULENAME, out grantedPermissionSetIds);
            List<BPInstance> bpInstances = dataManager.GetBeforeId(input, grantedPermissionSetIds);
            List<BPInstanceDetail> bpInstanceDetails = new List<BPInstanceDetail>();
            foreach (BPInstance bpInstance in bpInstances)
            {
                bpInstanceDetails.Add(BPInstanceDetailMapper(bpInstance));
            }
            return bpInstanceDetails;
        }

        public List<BPInstance> GetAfterId(long? processInstanceId, Guid bpDefinitionId)
        {
            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
            return dataManager.GetAfterId(processInstanceId, bpDefinitionId);
        }

        public bool HasRunningInstances(Guid definitionId, List<string> entityIds)
        {
            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
            return dataManager.HasRunningInstances(definitionId, entityIds, BPInstanceStatusAttribute.GetNonClosedStatuses());
        }
        public CreateProcessOutput CreateNewProcess(CreateProcessInput createProcessInput)
        {
            return CreateNewProcess(createProcessInput, false);
        }

        public BPInstanceDefinitionDetail GetBPInstanceDefinitionDetail(Guid bpDefinitionId, long bpInstanceId)
        {
            var bpdefinition = s_definitionManager.GetBPDefinition(bpDefinitionId);
            return BPInstanceDefinitionDetailMapper(bpdefinition, bpInstanceId);
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

            int? viewInstanceRequiredPermissionSetId = null;
            RequiredPermissionSettings viewInstanceRequiredPermissions = bpDefinitionManager.GetViewInstanceRequiredPermissions(processDefinition, createProcessInput.InputArguments);
            if (viewInstanceRequiredPermissions != null && viewInstanceRequiredPermissions.Entries != null && viewInstanceRequiredPermissions.Entries.Count > 0)
                viewInstanceRequiredPermissionSetId = new RequiredPermissionSetManager().GetRequiredPermissionSetId(BPInstance.REQUIREDPERMISSIONSET_MODULENAME, viewInstanceRequiredPermissions);

            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
            string processTitle = createProcessInput.InputArguments.GetTitle();
            if (processTitle != null)
                processTitle = processTitle.Replace("#BPDefinitionTitle#", processDefinition.Title);

            long processInstanceId = dataManager.InsertInstance(processTitle, createProcessInput.ParentProcessID, createProcessInput.CompletionNotifier, processDefinition.BPDefinitionID, createProcessInput.InputArguments,
                BPInstanceStatus.New, createProcessInput.InputArguments.UserId, createProcessInput.InputArguments.EntityId, viewInstanceRequiredPermissionSetId, createProcessInput.TaskId);
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
            {
                VRActionLogger.Current.LogObjectCustomAction(new BPInstanceLoggableEntity(createProcessInput.InputArguments.GetDefinitionTitle()), "Start Process", false, bpInstance);
            }
            CreateProcessOutput output = new CreateProcessOutput
            {
                ProcessInstanceId = processInstanceId,
                Result = CreateProcessResult.Succeeded
            };
            return output;
        }

        public UpdateOperationOutput<object> CancelProcess(long bpInstanceId)
        {
            int cancelRequestByUserId = SecurityContext.Current.GetLoggedInUserId();
            List<BPInstanceStatus> allowedStatuses = BPInstanceStatusAttribute.GetNonClosedStatuses();
            s_dataManager.SetCancellationRequestUserId(bpInstanceId, allowedStatuses, cancelRequestByUserId);
            var bpInstance = GetBPInstance(bpInstanceId);
            bpInstance.ThrowIfNull("bpInstance", bpInstanceId);
            if (bpInstance.CancellationRequestByUserId == cancelRequestByUserId)
                return new UpdateOperationOutput<object>
                {
                    Result = UpdateOperationResult.Succeeded
                };
            else
                return new UpdateOperationOutput<object>
                {
                    Result = UpdateOperationResult.Failed,
                    Message = String.Format("Process cannot be cancelled because it is in status '{0}'", Utilities.GetEnumDescription(bpInstance.Status)),
                    ShowExactMessage = true
                };
        }

        public bool DoesUserHaveCancelAccess(int userId, long bpInstanceId)
        {
            var bpInstance = GetBPInstance(bpInstanceId);
            bpInstance.ThrowIfNull("bpInstance", bpInstanceId);
            bpInstance.InputArgument.ThrowIfNull("bpInstance.InputArgument", bpInstanceId);
            var bpDefinition = s_definitionManager.GetBPDefinition(bpInstance.DefinitionID);
            bpDefinition.ThrowIfNull("bpDefinition", bpInstance.DefinitionID);
            return s_definitionManager.DoesUserHaveViewAccess(userId, bpDefinition, bpInstance.InputArgument)
                && s_definitionManager.DoesUserHaveStartNewInstanceAccess(userId, bpInstance.InputArgument);
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
            public BPInstanceLoggableEntity(string entityDisplayName)
            {
                _entityDisplayName = entityDisplayName;
            }

            static BPInstanceManager s_bpInstanceManager = new BPInstanceManager();
            public override string EntityUniqueName
            {
                get { return "VR_BusinessProcess_BPInstance"; }
            }

            public override string ModuleName
            {
                get { return "Business Process"; }
            }

            public override string EntityDisplayName
            {
                get { return this._entityDisplayName; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_BusinessProcess_BPInstance_ViewHistoryItem"; }
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

        public List<BPDefinitionSummary> GetBPDefinitionSummary()
        {
            IBPInstanceDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
            return dataManager.GetBPDefinitionSummary(BPInstanceStatusAttribute.GetNonClosedStatuses());
        }
        #endregion

        #region mapper

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

        private BPInstanceDefinitionDetail BPInstanceDefinitionDetailMapper(BPDefinition bpDefinition,long bpInstanceId)
        {
            BPInstanceDefinitionDetail detail = new BPInstanceDefinitionDetail();
            detail.Entity = bpDefinition;
            var extendedSettings = s_definitionManager.GetBPDefinitionExtendedSettings(bpDefinition);
            detail.AllowCancel = extendedSettings.CanCancelBPInstance(null) && DoesUserHaveCancelAccess(SecurityContext.Current.GetLoggedInUserId(), bpInstanceId);
            return detail;
        }

        #endregion
    }
}
