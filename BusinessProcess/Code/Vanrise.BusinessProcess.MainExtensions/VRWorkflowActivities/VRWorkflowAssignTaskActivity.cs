﻿using System;
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
        public override Guid ConfigId => new Guid("A9445F70-1188-4F39-8135-419954E88A8B");

        public override string Editor => throw new NotImplementedException();

        public override string Title => "Human Activity";

        public Guid TaskTypeId { get; set; }

        public string TaskTitle { get; set; }

        public List<VRWorkflowAssignTaskActivityInputItem> InputItems { get; set; }

        public List<VRWorkflowAssignTaskActivityOutputItem> OutputItems { get; set; }

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
                        #TASKDATARECORDTYPERUNTIMETYPE# TaskData;

                        public #CLASSNAME#ExecutionContext(NativeActivityContext activityContext) 
                            : this (activityContext, null)
                        {
                        }

                        public #CLASSNAME#ExecutionContext(NativeActivityContext activityContext, Vanrise.BusinessProcess.Entities.BPTask task) 
                            : base (activityContext)
                        {
                            _activityContext = activityContext;
                            if(task != null)
                            {
                                var genericTaskData = task.TaskData.CastWithValidate<Vanrise.BusinessProcess.MainExtensions.BPTaskType.BPGenericTaskData>(""task.TaskData"", task.BPTaskId);
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
                                throw new Exception(String.Format(""Could not resolve AssignedTo '{0}'"", AssignedTo));

                            BPTaskManager bpTaskManager = new BPTaskManager();
                            var createBPTaskInput = new CreateBPTaskInput
                            {
                                ProcessInstanceId = sharedData.InstanceInfo.ProcessInstanceID,
                                Title = GetTaskTitle(),
                                TaskData = taskData,
                                TaskName = taskData.TaskType,
                                AssignedUserIds = assignedUserIds.ToList(),
                                AssignedUserIdsDescription = assignedTo.GetDescription(bpTaskAssigneeContext),
                            };

                            var createTaskOutput = bpTaskManager.CreateTask(createBPTaskInput);
                            if (createTaskOutput != null && createTaskOutput.Result == CreateBPTaskResult.Succeeded)
                            {
                                context.CreateBookmark(createTaskOutput.WFBookmarkName, OnTaskCompleted);
                            }
                            else
                            {
                                throw new Exception(String.Format(""Could not create Task. Title '{0}'"", this.TaskTitle.Get(context)));
                            }
                        }

                        Vanrise.BusinessProcess.MainExtensions.BPTaskType.BPGenericTaskData GetTaskData()
                        {
                            #BUILDTASKDATA#
                            return Vanrise.BusinessProcess.MainExtensions.BPTaskType.BPGenericTaskData
                            {
                                TaskTypeId = new Guid(""#TASKTYPEID#""),
                                FieldValues = this.TaskData.GetDictionaryFromDataRecordType()
                            };
                        }

                        Vanrise.BusinessProcess.Entities.BPTaskAssignee GetTaskAssignee()
                        {
                            return new Vanrise.BusinessProcess.MainExtensions.InitiatorBPTaskAssignee();
                        }

                        string GetTaskTitle()
                        {
                            return #TASKTITLECODE#;
                        }

                        public void OnTaskCompleted()
                        {
                            #TASKCOMPLETEDCODE#
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

            if (this.InputItems != null && this.InputItems.Count > 0)
            {
                StringBuilder inputItemsBuilder = new StringBuilder();
                foreach (var prm in this.InputItems)
                {
                    inputItemsBuilder.AppendLine($"TaskData.{prm.FieldName} = {prm.Value};");
                }
                nmSpaceCodeBuilder.Replace("#BUILDTASKDATA#", inputItemsBuilder.ToString());
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#BUILDTASKDATA#", "");
            }

            if (this.OutputItems != null && this.OutputItems.Count > 0)
            {
                StringBuilder outputItemsBuilder = new StringBuilder();
                foreach (var prm in this.OutputItems)
                {
                    outputItemsBuilder.AppendLine($"{prm.To} = {prm.FieldName};");
                }
                nmSpaceCodeBuilder.Replace("#TASKCOMPLETEDCODE#", outputItemsBuilder.ToString());
            }
            else
            {
                nmSpaceCodeBuilder.Replace("#TASKCOMPLETEDCODE#", "");
            }

            nmSpaceCodeBuilder.Replace("#TASKTITLECODE#", this.TaskTitle);

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
    }

    public class VRWorkflowAssignTaskActivityInputItem
    {
        public string FieldName { get; set; }

        public string Value { get; set; }
    }

    public class VRWorkflowAssignTaskActivityOutputItem
    {
        public string To { get; set; }

        public string FieldName { get; set; }
    }

    public class VRWorkflowTaskAssignees
    {
        public VRWorkflowTaskAssigneesSetting Settings { get; set; }
    }

    public abstract class VRWorkflowTaskAssigneesSetting
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetAssigneeCode();
    }
}
