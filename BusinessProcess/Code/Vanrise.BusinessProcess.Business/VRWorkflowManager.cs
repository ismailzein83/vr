using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
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
			Func<VRWorkflow, bool> filterExpression = (item) =>
			{
				if (filter != null && filter.ExcludedIds != null && filter.ExcludedIds.Contains(item.VRWorkflowId))
					return false;

				return true;
			};

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

		public string GetVRWorkflowNameById(Guid vrWorkflowId)
		{
			VRWorkflow vrWorkflow = GetVRWorkflow(vrWorkflowId);
			vrWorkflow.ThrowIfNull("vrWorkflow", vrWorkflow.VRWorkflowId);

			return vrWorkflow.Name;
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
            return manager.GetExtensionConfigurations<VRWorkflowActivityConfig>(VRWorkflowActivityConfig.EXTENSION_TYPE).OrderBy(itm => itm.Title);
		}

		public VRWorkflowCompilationOutput TryCompileWorkflow(VRWorkflow workflow)
		{
			System.Activities.Activity activity;
			Dictionary<Guid, List<string>> activitiesErrors;

			bool compilationResult = TryCompileWorkflow(workflow, out activity, out activitiesErrors);

			if (compilationResult)
			{
				return new VRWorkflowCompilationOutput
				{
					ActivitiesErrors = null,
					Result = true
				};
			}
			else
			{
				return new VRWorkflowCompilationOutput
				{
					ActivitiesErrors = activitiesErrors,
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
				  Dictionary<Guid, List<string>> activitiesErrors;

				  if (TryCompileWorkflow(vrWorkflow, out workflowActivity, out activitiesErrors))
					  return workflowActivity;
				  else
				  {
					  StringBuilder errorsBuilder = new StringBuilder();
					  foreach (var activityErrorsKvp in activitiesErrors)
					  {
						  List<string> errorMessages = activityErrorsKvp.Value;
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

		public bool TryCompileWorkflow(VRWorkflow workflow, out System.Activities.Activity activity, out Dictionary<Guid, List<string>> activitiesErrors)
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
			generateCodeContext.VRWorkflowActivityId = workflow.Settings.RootActivity.VRWorkflowActivityId;
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
			List<string> errorMessages;
			if (CSharpCompiler.TryCompileClass(className, codeBuilder.ToString(), out compilationOutput))
			{
				Type activityType = compilationOutput.OutputAssembly.GetType(fullTypeName);
				activity = Activator.CreateInstance(activityType).CastWithValidate<System.Activities.Activity>("activity", workflow.VRWorkflowId);
				errorMessages = null;
				activitiesErrors = null;
				return true;
			}
			else
			{
				activity = null;
				errorMessages = compilationOutput.ErrorMessages;
				var errors = compilationOutput.Errors;
				activitiesErrors = SplitErrorsByActivities(codeBuilder, errors);
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

		private Dictionary<Guid, List<string>> SplitErrorsByActivities(StringBuilder codeBuilder, List<CSharpCompilationError> errors)
		{
			if (errors == null || errors.Count == 0)
				return null;

			Dictionary<Guid, List<string>> activitiesErrors = new Dictionary<Guid, List<string>>();

			string[] codeLines = codeBuilder.ToString().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None);

			foreach (var error in errors)
			{
				var errorLineNumber = error.LineNumber;
				for (int i = errorLineNumber - 2; i >= 0; i--)
				{
					string currentCodeLine = codeLines[i];
					if (currentCodeLine.StartsWith("#region"))
					{
						Guid faultyWorkflowActivityId = new Guid(currentCodeLine.Replace("#region", "").Trim());
						List<string> activityErrors = activitiesErrors.GetOrCreateItem(faultyWorkflowActivityId, () => { return new List<string>(); });
						activityErrors.Add(error.ErrorText);
						break;
					}
				}
			}

			return activitiesErrors;
		}

		#endregion

		#region Internal/Private Classes

		internal class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			IVRWorkflowDataManager dataManager = BPDataManagerFactory.GetDataManager<IVRWorkflowDataManager>();
			object _updateHandle;

			protected override bool ShouldSetCacheExpired(object parameter)
			{
				return dataManager.AreVRWorkflowsUpdated(ref _updateHandle);
			}
		}

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

		private class VRWorkflowActivityGenerateWFActivityCodeContext : IVRWorkflowActivityGenerateWFActivityCodeContext
		{
			public Guid VRWorkflowActivityId { get; set; }

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
				StringBuilder strBuilder = new StringBuilder();
				strBuilder.AppendLine(string.Empty);
				strBuilder.AppendLine(string.Format("#region {0}", VRWorkflowActivityId));
				strBuilder.AppendLine(namespaceCode);
				strBuilder.AppendLine("#endregion");

				this.OtherNameSpaceCodes.Add(strBuilder.ToString());
			}

			public void AddUsingStatement(string usingStatement)
			{
				StringBuilder strBuilder = new StringBuilder();
				strBuilder.AppendLine(string.Empty);
				strBuilder.AppendLine(string.Format("#region {0}", VRWorkflowActivityId));
				strBuilder.AppendLine(usingStatement);
				strBuilder.AppendLine("#endregion");

				this.AdditionalUsingStatements.Add(strBuilder.ToString());
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
		public Dictionary<Guid, List<string>> ActivitiesErrors { get; set; }
		public bool Result { get; set; }
	}
}