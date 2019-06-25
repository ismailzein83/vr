using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.MainExtensions.BPTaskTypes;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowAssignTaskActivity : VRWorkflowActivitySettings
    {
        public VRWorkflowAssignTaskActivity()
        {
            this.EnableVisualization = true;
        }

        public override Guid ConfigId { get { return new Guid("A9445F70-1188-4F39-8135-419954E88A8B"); } }

        public override string Editor { get { return "businessprocess-vr-workflowactivity-assigntask"; } }

        public override string Title { get { return "Human Activity"; } }

        public Guid TaskTypeId { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(VRWorkflowExpressionJsonConverter))]
        public VRWorkflowExpression TaskTitle { get; set; }

        public string DisplayName { get; set; }

        public VRWorkflowTaskAssignees TaskAssignees { get; set; }

        public VRWorkflowExpression ExecutedBy { get; set; }
        
        public List<VRWorkflowAssignTaskActivityInputItem> InputItems { get; set; }

        public List<VRWorkflowAssignTaskActivityOutputItem> OutputItems { get; set; }

        public bool EnableVisualization { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder nmSpaceCodeBuilder = new StringBuilder(@"                 

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : NativeActivity
                    {
                        protected override void Execute(NativeActivityContext context)
                        {
                            var executionContext = new #CLASSNAME#ExecutionContext(context);
                            executionContext.Execute(OnTaskCompleted);
                        }

                        private void OnTaskCompleted(NativeActivityContext context, Bookmark bookmark, object value)
                        {
                            Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput executeBPTaskInput = value as Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput;
                            if (executeBPTaskInput == null)
                                throw new ArgumentNullException(""ExecuteBPTaskInput"");
                            Vanrise.BusinessProcess.Entities.BPTask task;
                            Vanrise.BusinessProcess.Business.BPTaskManager bpTaskManager = new Vanrise.BusinessProcess.Business.BPTaskManager();
                            bpTaskManager.SetTaskCompleted(executeBPTaskInput, out task);

                            var executionContext = new #CLASSNAME#ExecutionContext(context, task);
                            executionContext.OnTaskCompleted();
                        }

                        protected override bool CanInduceIdle
                        {
                            get
                            {
                                return true;
                            }
                        }
                    }

                    #BASEEXECUTIONCLASSCODE#                  

                    public class #CLASSNAME#ExecutionContext : #BASEEXECUTIONCLASSNAME#
                    {
                        NativeActivityContext _activityContext;
                        Vanrise.BusinessProcess.Entities.BPTask _task;
                        #TASKDATARECORDTYPERUNTIMETYPE# TaskData;

                        public #CLASSNAME#ExecutionContext(NativeActivityContext activityContext) 
                            : this (activityContext, null)
                        {
                        }

                        public #CLASSNAME#ExecutionContext(NativeActivityContext activityContext, Vanrise.BusinessProcess.Entities.BPTask task) 
                            : base (activityContext)
                        {
                            _activityContext = activityContext;
                            _task = task;
                            if(task != null)
                            {
                                var genericTaskData = task.TaskData.CastWithValidate<Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.BPGenericTaskData>(""task.TaskData"", task.BPTaskId);
                                this.TaskData = new #TASKDATARECORDTYPERUNTIMETYPE#(genericTaskData.FieldValues);
                            }
                            else
                            {
                                this.TaskData = new #TASKDATARECORDTYPERUNTIMETYPE#();
                            }
                        }

                        public void Execute(BookmarkCallback OnTaskCompleted)
                        {

                            var sharedData = _activityContext.GetSharedInstanceData();

                            var taskData = GetTaskData();
                            var assignedTo = GetTaskAssignee();

                            Vanrise.BusinessProcess.Business.BPTaskAssigneeContext bpTaskAssigneeContext = new Vanrise.BusinessProcess.Business.BPTaskAssigneeContext() { ProcessInitiaterUserId = sharedData.InstanceInfo.InitiatorUserId };

                            var assignedUserIds = assignedTo.GetUserIds(bpTaskAssigneeContext);
                            if (assignedUserIds == null || assignedUserIds.Count() == 0)
                                throw new Exception(String.Format(""Could not resolve AssignedTo""));

                            Vanrise.BusinessProcess.Business.BPTaskManager bpTaskManager = new Vanrise.BusinessProcess.Business.BPTaskManager();
                            var createBPTaskInput = new Vanrise.BusinessProcess.Entities.CreateBPTaskInput
                            {
                                ProcessInstanceId = sharedData.InstanceInfo.ProcessInstanceID,
                                Title = GetTaskTitle(),
                                TaskData = taskData,
                                TaskName = taskData.TaskType,
                                AssignedUserIds = assignedUserIds.ToList(),
                                AssignedUserIdsDescription = assignedTo.GetDescription(bpTaskAssigneeContext),
                            };

                            var createTaskOutput = bpTaskManager.CreateTask(createBPTaskInput);
                            if (createTaskOutput != null && createTaskOutput.Result == Vanrise.BusinessProcess.Entities.CreateBPTaskResult.Succeeded)
                            {
                                _activityContext.CreateBookmark(createTaskOutput.WFBookmarkName, OnTaskCompleted);
                                WriteInformation(""'#TASKTITLE#' started"");
                                #TASKSTARTEDVISUALEVENT#
                            }
                            else
                            {
                                throw new Exception(String.Format(""Could not create Task. Title '{0}'"", GetTaskTitle()));
                            }
                        }

                        Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.BPGenericTaskData GetTaskData()
                        {
                            #BUILDTASKDATA#
                            return new Vanrise.BusinessProcess.MainExtensions.BPTaskTypes.BPGenericTaskData
                            {
                                TaskTypeId = new Guid(""#TASKTYPEID#""),
                                FieldValues = this.TaskData.GetDictionaryFromDataRecordType()
                            };
                        }

                        Vanrise.BusinessProcess.Entities.BPTaskAssignee GetTaskAssignee()
                        {
                            #TASKASSIGNEES#
                        }

                        string GetTaskTitle()
                        {
                            return #TASKTITLECODE#;
                        }

                        public void OnTaskCompleted()
                        {
                            #TASKCOMPLETEDCODE#
                            WriteInformation(""'#TASKTITLE#' completed"");
                            #TASKCOMPLETEDVISUALEVENT#
                        }
                    }
                }");

            nmSpaceCodeBuilder.Replace("#TASKTYPEID#", this.TaskTypeId.ToString());

            var taskTypeManager = new BPTaskTypeManager();
            var taskType = taskTypeManager.GetBPTaskType(this.TaskTypeId);
            taskType.ThrowIfNull("taskType", this.TaskTypeId);

            BPGenericTaskTypeSettings genericTaskTypeSettings = taskType.Settings.CastWithValidate<BPGenericTaskTypeSettings>("taskType.Settings", this.TaskTypeId);

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(genericTaskTypeSettings.RecordTypeId);
            dataRecordRuntimeType.ThrowIfNull("dataRecordRuntimeType", genericTaskTypeSettings.RecordTypeId);

            nmSpaceCodeBuilder.Replace("#TASKDATARECORDTYPERUNTIMETYPE#", CSharpCompiler.TypeToString(dataRecordRuntimeType));
            nmSpaceCodeBuilder.Replace("#TASKTITLE#", this.DisplayName);


            if (this.InputItems != null && this.InputItems.Count > 0)
            {
                StringBuilder inputItemsBuilder = new StringBuilder();
                foreach (var prm in this.InputItems)
                {
                    inputItemsBuilder.AppendLine($"TaskData.{prm.FieldName} = {prm.Value.GetCode(null)};");
                }
                nmSpaceCodeBuilder.Replace("#BUILDTASKDATA#", inputItemsBuilder.ToString());
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#BUILDTASKDATA#", "");
            }

            if ((this.OutputItems != null && this.OutputItems.Count > 0)
                || this.ExecutedBy != null)
            {
                StringBuilder outputItemsBuilder = new StringBuilder();
                foreach (var prm in this.OutputItems)
                {
                    outputItemsBuilder.AppendLine($"{prm.To.GetCode(null)} = TaskData.{prm.FieldName};");
                }

                string executedByCode = this.ExecutedBy != null ? this.ExecutedBy.GetCode(null) : null;
                if (!string.IsNullOrEmpty(executedByCode))
                    outputItemsBuilder.AppendLine($"{executedByCode} = _task.ExecutedById.Value;");

                nmSpaceCodeBuilder.Replace("#TASKCOMPLETEDCODE#", outputItemsBuilder.ToString());
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#TASKCOMPLETEDCODE#", "");
            }

            nmSpaceCodeBuilder.Replace("#TASKTITLECODE#", this.TaskTitle.GetCode(null));
            this.TaskAssignees.ThrowIfNull("this.TaskAssignees");
            this.TaskAssignees.Settings.ThrowIfNull("this.TaskAssignees.Settings");

            string taskAssigneesCode = this.TaskAssignees.Settings.GetAssigneesCode();
            nmSpaceCodeBuilder.Replace("#TASKASSIGNEES#", taskAssigneesCode);

            if(this.EnableVisualization)
            {
                var startedVisualEventInput = new GenerateInsertVisualEventCodeInput
                {
                    ActivityContextVariableName = "_activityContext",
                    ActivityId = context.VRWorkflowActivityId,
                    EventTitle = $@"""Task '{this.DisplayName}' started""",
                    EventTypeId = CodeGenerationHelper.VISUALEVENTTYPE_STARTED,
                    EventPayloadCode = @"
                        new Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BPHumanTaskStartedVisualEventPayload
                        {
                            TaskId = createTaskOutput.TaskId
                        }"
                };
                nmSpaceCodeBuilder.Replace("#TASKSTARTEDVISUALEVENT#",
                    context.GenerateInsertVisualEventCode(startedVisualEventInput));

                var completedVisualEventInput = new GenerateInsertVisualEventCodeInput
                {
                    ActivityContextVariableName = "_activityContext",
                    ActivityId = context.VRWorkflowActivityId,
                    EventTitle = $@"""Task '{this.DisplayName}' completed""",
                    EventTypeId = CodeGenerationHelper.VISUALEVENTTYPE_COMPLETED
                };
                nmSpaceCodeBuilder.Replace("#TASKCOMPLETEDVISUALEVENT#",
                    context.GenerateInsertVisualEventCode(completedVisualEventInput));
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#TASKSTARTEDVISUALEVENT#", "");
                nmSpaceCodeBuilder.Replace("#TASKCOMPLETEDVISUALEVENT#", "");
            }

            string baseExecutionContextClassName = "AssignTaskActivity_BaseExecutionContext";
            string baseExecutionContextClassCode = CodeGenerationHelper.GenerateBaseExecutionClass(context, baseExecutionContextClassName);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSCODE#", baseExecutionContextClassCode);
            nmSpaceCodeBuilder.Replace("#BASEEXECUTIONCLASSNAME#", baseExecutionContextClassName);
            string codeNamespace = context.GenerateUniqueNamespace("AssignTaskActivity");
            string className = "AssignTaskActivity";
            string fullTypeName = string.Concat(codeNamespace, ".", className);

            nmSpaceCodeBuilder.Replace("#NAMESPACE#", codeNamespace);
            nmSpaceCodeBuilder.Replace("#CLASSNAME#", className);
            context.AddFullNamespaceCode(nmSpaceCodeBuilder.ToString());

            return string.Format("new {0}()", fullTypeName);
        }

        public override BPVisualItemDefinition GetVisualItemDefinition(IVRWorkflowActivityGetVisualItemDefinitionContext context)
        {
            if(this.EnableVisualization)
            {
                string displayName = this.DisplayName;
                if (context.SubProcessActivityName != null)
                    displayName = displayName.Replace("[SubProcessActivityName]", context.SubProcessActivityName);
                return new BPVisualItemDefinition
                {                    
                    Settings = new BPHumanTaskVisualItemDefinitionSettings { TaskName = displayName }
                };
            }
            else
            {
                return null;
            }
        }
    }

    public class VRWorkflowAssignTaskActivityInputItem
    {
        public string FieldName { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(VRWorkflowExpressionJsonConverter))]
        public VRWorkflowExpression Value { get; set; }
    }

    public class VRWorkflowAssignTaskActivityOutputItem
    {
        [Newtonsoft.Json.JsonConverter(typeof(VRWorkflowExpressionJsonConverter))]
        public VRWorkflowExpression To { get; set; }

        public string FieldName { get; set; }
    }
    
    public class BPHumanTaskVisualItemDefinitionSettings : BPVisualItemDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("BA33DC7B-1357-47B5-A256-0044ED93EEAE"); } }

        public override string Editor { get { return "bp-workflow-activitysettings-visualitemdefiniton-humantask"; } }

        public string TaskName { get; set; }
    }

    public class BPHumanTaskStartedVisualEventPayload : BPVisualEventPayload
    {
        public long TaskId { get; set; }
    }
}
