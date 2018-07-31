using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class VRWorkflowManager : IVRWorkflowManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<VRWorkflowDetail> GetFilteredVRWorkflows(Vanrise.Entities.DataRetrievalInput<VRWorkflowQuery> input)
        {
            var allVRWorkflows = GetCachedVRWorkflows();

            Func<VRWorkflow, bool> filterExpression = (prod) =>
            {
                if (input.Query != null)
                {
                    if (!string.IsNullOrEmpty(input.Query.Name) && !prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                        return false;

                    if (!string.IsNullOrEmpty(input.Query.Title) && !prod.Title.ToLower().Contains(input.Query.Title.ToLower()))
                        return false;
                }

                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(VRWorkflowLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRWorkflows.ToBigResult(input, filterExpression, VRWorkflowDetailMapper));
        }

        public IEnumerable<VRWorkflowInfo> GetVRWorkflowsInfo(VRWorkflowFilter filter)
        {
            Func<VRWorkflow, bool> filterExpression = null;

            return this.GetCachedVRWorkflows().MapRecords(VRWorkflowInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public InsertOperationOutput<VRWorkflowDetail> InsertVRWorkflow(VRWorkflowToAdd vrWorkflowToAdd)
        {
            IVRWorkflowDataManager dataManager = BPDataManagerFactory.GetDataManager<IVRWorkflowDataManager>();

            Guid vrWorkflowId = Guid.NewGuid();
            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            int createdBy = loggedInUserId;

            bool insertActionSucc = dataManager.InsertVRWorkflow(vrWorkflowToAdd, vrWorkflowId, createdBy);

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRWorkflowDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

                VRWorkflow insertedWorkflow = GetVRWorkflow(vrWorkflowId);

                VRActionLogger.Current.TrackAndLogObjectAdded(VRWorkflowLoggableEntity.Instance, insertedWorkflow);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRWorkflowDetailMapper(insertedWorkflow);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<VRWorkflowDetail> UpdateVRWorkflow(VRWorkflowToUpdate vrWorkflow)
        {
            IVRWorkflowDataManager dataManager = BPDataManagerFactory.GetDataManager<IVRWorkflowDataManager>();

            int lastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();

            bool updateActionSucc = dataManager.UpdateVRWorkflow(vrWorkflow, lastModifiedBy);
            UpdateOperationOutput<VRWorkflowDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRWorkflowDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

                VRWorkflow updatedWorkflow = GetVRWorkflow(vrWorkflow.VRWorkflowId);

                VRActionLogger.Current.TrackAndLogObjectUpdated(VRWorkflowLoggableEntity.Instance, updatedWorkflow);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRWorkflowDetailMapper(updatedWorkflow);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public VRWorkflow GetVRWorkflow(Guid vrWorkflowId)
        {
            var allVRWorkflows = GetCachedVRWorkflows();
            if (allVRWorkflows == null)
                return null;

            return allVRWorkflows.GetRecord(vrWorkflowId);
        }

        public string GetVRWorkflowName(VRWorkflow vrWorkflow)
        {
            return (vrWorkflow != null) ? vrWorkflow.Name : null;
        }

        public VRWorkflowEditorRuntime GetVRWorkflowEditorRuntime(Guid vrWorkflowId)
        {
            VRWorkflow vrWorkflow = GetVRWorkflow(vrWorkflowId);
            if (vrWorkflow == null)
                vrWorkflow.ThrowIfNull("vrWorkflow", vrWorkflow.VRWorkflowId);

            var argumentEditorRuntimeDic = new Dictionary<Guid, VRWorkflowArgumentEditorRuntime>();

            if (vrWorkflow.Settings != null && vrWorkflow.Settings.Arguments != null)
            {
                foreach (var argument in vrWorkflow.Settings.Arguments)
                {
                    if (argument.Type == null)
                        argument.Type.ThrowIfNull("argument.Type", argument.VRWorkflowArgumentId);

                    argumentEditorRuntimeDic.Add(argument.VRWorkflowArgumentId, new VRWorkflowArgumentEditorRuntime
                    {
                        VRWorkflowVariableTypeDescription = argument.Type.GetRuntimeTypeDescription()
                    });
                }
            }

            return new VRWorkflowEditorRuntime
            {
                Entity = vrWorkflow,
                VRWorkflowArgumentEditorRuntimeDict = argumentEditorRuntimeDic.Count > 0 ? argumentEditorRuntimeDic : null
            };
        }

        public Dictionary<Guid, string> GetVRWorkflowVariablesTypeDescription(IEnumerable<VRWorkflowVariable> variables)
        {
            if (variables == null || !variables.Any())
                return null;
            var result = new Dictionary<Guid, string>();

            foreach (var variable in variables)
            {
                if (variable.Type == null)
                    variable.Type.ThrowIfNull("argument.Type", variable.VRWorkflowVariableId);

                result.Add(variable.VRWorkflowVariableId, variable.Type.GetRuntimeTypeDescription());
            }
            return result;
        }

        public string GetVRWorkflowArgumentTypeDescription(VRWorkflowVariableType vrWorkflowArgumentType)
        {
            return vrWorkflowArgumentType.GetRuntimeTypeDescription();
        }

        public string GetVRWorkflowVariableTypeDescription(VRWorkflowVariableType vrWorkflowVariableType)
        {
            return vrWorkflowVariableType.GetRuntimeTypeDescription();
        }

        public IEnumerable<VRWorkflowVariableTypeConfig> GetVRWorkflowVariableTypeExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<VRWorkflowVariableTypeConfig>(VRWorkflowVariableTypeConfig.EXTENSION_TYPE);
        }

        public IEnumerable<VRWorkflowActivityConfig> GetVRWorkflowActivityExtensionConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<VRWorkflowActivityConfig>(VRWorkflowActivityConfig.EXTENSION_TYPE);
        }

        public VRWorkflowCompilationOutput TryCompileWorkflow(VRWorkflow workflow)
        {
            System.Activities.Activity activity;
            List<string> errorMessages;
            bool compilationResult = TryCompileWorkflow(workflow, out activity, out errorMessages);

            if (compilationResult)
            {
                return new VRWorkflowCompilationOutput
                {
                    ErrorMessages = null,
                    Result = true
                };
            }
            else
            {
                return new VRWorkflowCompilationOutput
                {
                    ErrorMessages = errorMessages,
                    Result = false
                };
            }
        }

        public System.Activities.Activity GetVRWorkflowActivity(Guid vrWorkflowId)
        {
            var cacheName = new GetVRWorkflowActivityCacheName { VRWorkflowId = vrWorkflowId };
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
              () =>
              {
                  var vrWorkflow = GetVRWorkflow(vrWorkflowId);
                  if (vrWorkflow == null)
                      throw new NullReferenceException(String.Format("vrWorkflow '{0}'", vrWorkflowId));
                  Activity workflowActivity;
                  List<string> errorMessages;

                  if (TryCompileWorkflow(vrWorkflow, out workflowActivity, out errorMessages))
                      return workflowActivity;
                  else
                  {
                      StringBuilder errorsBuilder = new StringBuilder();
                      if (errorMessages != null)
                      {
                          foreach (var errorMessage in errorMessages)
                              errorsBuilder.AppendLine(errorMessage);
                      }
                      throw new Exception(String.Format("Compile Error when building for VRWorkflow Id '{0}'. Errors: {1}",
                          vrWorkflowId, errorsBuilder));
                  }
              });
        }

        private struct GetVRWorkflowActivityCacheName
        {
            public Guid VRWorkflowId { get; set; }
        }

        private bool TryCompileWorkflow(VRWorkflow workflow, out System.Activities.Activity activity, out List<string> errorMessages)
        {
            StringBuilder codeBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using Vanrise.Common;
                using Vanrise.BusinessProcess;
                using System.Activities;
                using System.Activities.Statements;
                using Microsoft.CSharp.Activities;
                #ADDITIONALUSINGSTATEMENTS#

                #OTHERNAMESPACES#

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : Activity
                    {
                        #ARGUMENTS#

                        public #CLASSNAME#()
                        {
                            this.Implementation = () => #ROOTACTIVITY#;
                        }
                    }
                }");

            workflow.ThrowIfNull("workflow");
            workflow.Settings.ThrowIfNull("workflow.Settings", workflow.VRWorkflowId);
            workflow.Settings.RootActivity.ThrowIfNull("workflow.Settings.RootActivity", workflow.VRWorkflowId);
            workflow.Settings.RootActivity.Settings.ThrowIfNull("workflow.Settings.RootActivity.Settings", workflow.VRWorkflowId);
            var generateCodeContext = new VRWorkflowActivityGenerateWFActivityCodeContext(workflow.Settings.Arguments);
            string rootActivityCode = workflow.Settings.RootActivity.Settings.GenerateWFActivityCode(generateCodeContext);
            codeBuilder.Replace("#ROOTACTIVITY#", rootActivityCode);

            if (workflow.Settings.Arguments != null)
            {
                StringBuilder argumentsCodeBuilder = new StringBuilder();
                foreach (var argument in workflow.Settings.Arguments)
                {
                    string argumentRuntimeType = CSharpCompiler.TypeToString(argument.Type.GetRuntimeType(null));
                    argumentsCodeBuilder.AppendLine(string.Concat("public ", argument.Direction.ToString(), "Argument<", argumentRuntimeType, "> ", argument.Name, " { get; set; }"));
                }
                codeBuilder.Replace("#ARGUMENTS#", argumentsCodeBuilder.ToString());
            }
            else
            {
                codeBuilder.Replace("#ARGUMENTS#", "");
            }

            if (generateCodeContext.AdditionalUsingStatements != null)
            {
                var additionalUsingStatementsCodeBuilder = new StringBuilder();
                foreach (var otherNameSpaceCode in generateCodeContext.AdditionalUsingStatements)
                {
                    additionalUsingStatementsCodeBuilder.AppendLine(otherNameSpaceCode);
                }
                codeBuilder.Replace("#ADDITIONALUSINGSTATEMENTS#", additionalUsingStatementsCodeBuilder.ToString());
            }
            else
            {
                codeBuilder.Replace("#ADDITIONALUSINGSTATEMENTS#", "");
            }

            if (generateCodeContext.OtherNameSpaceCodes != null)
            {
                var otherNameSpacesCodeBuilder = new StringBuilder();
                foreach (var otherNameSpaceCode in generateCodeContext.OtherNameSpaceCodes)
                {
                    otherNameSpacesCodeBuilder.AppendLine(otherNameSpaceCode);
                }
                codeBuilder.Replace("#OTHERNAMESPACES#", otherNameSpacesCodeBuilder.ToString());
            }
            else
            {
                codeBuilder.Replace("#OTHERNAMESPACES#", "");
            }

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.BusinessProcess.Business.VRWorkflowManager");
            string className = string.Concat("VRWorkflow_", workflow.VRWorkflowId.ToString().Replace("-", ""));
            codeBuilder.Replace("#NAMESPACE#", classNamespace);
            codeBuilder.Replace("#CLASSNAME#", className);
            var fullTypeName = String.Format("{0}.{1}", classNamespace, className);
            CSharpCompilationOutput compilationOutput;
            if (CSharpCompiler.TryCompileClass(codeBuilder.ToString(), out compilationOutput))
            {
                Type activityType = compilationOutput.OutputAssembly.GetType(fullTypeName);
                activity = Activator.CreateInstance(activityType).CastWithValidate<System.Activities.Activity>("activity", workflow.VRWorkflowId);
                errorMessages = null;
                return true;
            }
            else
            {
                activity = null;
                errorMessages = compilationOutput.ErrorMessages;
                return false;
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<Guid, VRWorkflow> GetCachedVRWorkflows()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRWorkflows",
               () =>
               {
                   IVRWorkflowDataManager dataManager = BPDataManagerFactory.GetDataManager<IVRWorkflowDataManager>();
                   IEnumerable<VRWorkflow> vrWorkflows = dataManager.GetVRWorkflows();
                   return vrWorkflows.ToDictionary(record => record.VRWorkflowId, record => record);
               });
        }

        private bool TryCompileActivityExpressions(Activity dynamicActivity, out List<string> errorMessages)
        {
            TextExpressionCompilerSettings settings = new TextExpressionCompilerSettings
            {
                Activity = dynamicActivity,
                Language = "C#",
                ActivityName = dynamicActivity.GetType().FullName.Split('.').Last() + "_CompiledExpressionRoot",
                ActivityNamespace = string.Join(".", dynamicActivity.GetType().FullName.Split('.').Reverse().Skip(1).Reverse()),
                RootNamespace = null,
                GenerateAsPartialClass = false,
                AlwaysGenerateSource = true,
            };

            TextExpressionCompilerResults results = new TextExpressionCompiler(settings).Compile();

            if (results.HasErrors)
            {
                errorMessages = results.CompilerMessages.Where(itm => !itm.IsWarning).Select(itm => itm.Message).ToList();
                return false;
            }

            ICompiledExpressionRoot compiledExpressionRoot = Activator.CreateInstance(results.ResultType, new object[] { dynamicActivity }) as ICompiledExpressionRoot;
            CompiledExpressionInvoker.SetCompiledExpressionRootForImplementation(dynamicActivity, compiledExpressionRoot); errorMessages = null;
            return true;
        }

        #endregion

        #region Private Classes

        private class VRWorkflowLoggableEntity : VRLoggableEntityBase
        {
            public static VRWorkflowLoggableEntity Instance = new VRWorkflowLoggableEntity();

            private VRWorkflowLoggableEntity()
            {

            }

            static VRWorkflowManager s_vrWorkflowManager = new VRWorkflowManager();

            public override string EntityUniqueName
            {
                get { return "BusinessProcess_VR_Workflow"; }
            }

            public override string ModuleName
            {
                get { return "Business Process"; }
            }

            public override string EntityDisplayName
            {
                get { return "VRWorkflow"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "BusinessProcess_BP_VRWorkflow_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRWorkflow vrWorkflow = context.Object.CastWithValidate<VRWorkflow>("context.Object");
                return vrWorkflow.VRWorkflowId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRWorkflow vrWorkflow = context.Object.CastWithValidate<VRWorkflow>("context.Object");
                return s_vrWorkflowManager.GetVRWorkflowName(vrWorkflow);
            }
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRWorkflowDataManager dataManager = BPDataManagerFactory.GetDataManager<IVRWorkflowDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreVRWorkflowsUpdated(ref _updateHandle);
            }
        }

        private class VRWorkflowActivityGenerateWFActivityCodeContext : IVRWorkflowActivityGenerateWFActivityCodeContext
        {
            Dictionary<string, VRWorkflowArgument> _allArguments = new Dictionary<string, VRWorkflowArgument>();

            List<IVRWorkflowActivityGenerateWFActivityCodeContext> _childContexts = new List<IVRWorkflowActivityGenerateWFActivityCodeContext>();

            public List<string> OtherNameSpaceCodes { get; private set; }

            public List<string> AdditionalUsingStatements { get; private set; }

            Dictionary<string, VRWorkflowVariable> _allVariables = new Dictionary<string, VRWorkflowVariable>();

            public VRWorkflowActivityGenerateWFActivityCodeContext(VRWorkflowArgumentCollection workflowArguments)
            {
                if (workflowArguments != null)
                {
                    foreach (var arg in workflowArguments)
                    {
                        _allArguments.Add(arg.Name, arg);
                    }
                }
                this.OtherNameSpaceCodes = new List<string>();
                this.AdditionalUsingStatements = new List<string>();
            }

            private VRWorkflowActivityGenerateWFActivityCodeContext(VRWorkflowActivityGenerateWFActivityCodeContext parentContext)
            {
                if (parentContext._allVariables != null)
                {
                    foreach (var parentVariable in parentContext._allVariables)
                    {
                        AddVariable(parentVariable.Value);
                    }
                }
                this._allArguments = parentContext._allArguments;
                this.OtherNameSpaceCodes = parentContext.OtherNameSpaceCodes;
                this.AdditionalUsingStatements = parentContext.AdditionalUsingStatements;
            }

            public void AddVariables(VRWorkflowVariableCollection variables)
            {
                variables.ThrowIfNull("variables");
                foreach (var variable in variables)
                {
                    AddVariable(variable);
                }
            }

            public void AddVariable(VRWorkflowVariable variable)
            {
                variable.ThrowIfNull("variable");

                if (_allVariables.ContainsKey(variable.Name))
                    throw new Exception(String.Format("Variable '{0}' already added to the Workflow Variables", variable.Name));
                _allVariables.Add(variable.Name, variable);
                foreach (var childContext in this._childContexts)
                {
                    childContext.AddVariable(variable);
                }
            }

            public VRWorkflowVariable GetVariableWithValidate(string variableName)
            {
                VRWorkflowVariable variable = _allVariables.GetRecord(variableName);
                variable.ThrowIfNull("variable", variableName);
                return variable;
            }

            public string GenerateUniqueNamespace(string nmSpace)
            {
                return String.Concat(nmSpace, "_", Guid.NewGuid().ToString().Replace("-", ""));
            }

            public void AddFullNamespaceCode(string namespaceCode)
            {
                this.OtherNameSpaceCodes.Add(namespaceCode);
            }

            public void AddUsingStatement(string usingStatement)
            {
                this.AdditionalUsingStatements.Add(usingStatement);
            }


            public IEnumerable<VRWorkflowVariable> GetAllVariables()
            {
                return _allVariables.Values;
            }

            public IEnumerable<VRWorkflowArgument> GetAllWorkflowArguments()
            {
                return _allArguments.Values;
            }

            public IVRWorkflowActivityGenerateWFActivityCodeContext CreateChildContext()
            {
                return new VRWorkflowActivityGenerateWFActivityCodeContext(this);
            }
        }

        #endregion

        #region Mappers

        private VRWorkflowDetail VRWorkflowDetailMapper(VRWorkflow vrWorkflow)
        {
            if (vrWorkflow == null)
                return null;

            VRWorkflowDetail vrWorkflowDetail = new VRWorkflowDetail()
            {
                VRWorkflowID = vrWorkflow.VRWorkflowId,
                Name = vrWorkflow.Name,
                Title = vrWorkflow.Title,
                LastModifiedTime = vrWorkflow.LastModifiedTime
            };
            return vrWorkflowDetail;
        }

        private VRWorkflowInfo VRWorkflowInfoMapper(VRWorkflow vrWorkflow)
        {
            VRWorkflowInfo vrWorkflowInfo = new VRWorkflowInfo()
            {
                VRWorkflowId = vrWorkflow.VRWorkflowId,
                Name = vrWorkflow.Name
            };
            return vrWorkflowInfo;
        }

        #endregion
    }

    public class VRWorkflowCompilationOutput
    {
        public List<string> ErrorMessages { get; set; }
        public bool Result { get; set; }
    }
}