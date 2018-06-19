using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Business
{
    public class VRWorkflowManager
    {
        public bool TryCompileWorkflow(VRWorkflow workflow, out System.Activities.Activity activity, out List<string> errorMessages)
        {
            StringBuilder codeBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;
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

            if(workflow.Settings.Arguments != null)
            {
                StringBuilder argumentsCodeBuilder = new StringBuilder();
                foreach(var argument in workflow.Settings.Arguments)
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

            if(generateCodeContext.OtherNameSpaceCodes != null)
            {
                var otherNameSpacesCodeBuilder = new StringBuilder();
                foreach(var otherNameSpaceCode in generateCodeContext.OtherNameSpaceCodes)
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
                return TryCompileActivityExpressions(activity, out errorMessages);
            }
            else
            {
                activity = null;
                errorMessages = compilationOutput.ErrorMessages;
                return false;
            }
        }

        bool TryCompileActivityExpressions(Activity dynamicActivity, out List<string> errorMessages)
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



            TextExpressionCompilerResults results =

                new TextExpressionCompiler(settings).Compile();

            if (results.HasErrors)
            {
                errorMessages = results.CompilerMessages.Where(itm => !itm.IsWarning).Select(itm => itm.Message).ToList();
                return false;
            }



            ICompiledExpressionRoot compiledExpressionRoot =

                Activator.CreateInstance(results.ResultType,

                    new object[] { dynamicActivity }) as ICompiledExpressionRoot;

            CompiledExpressionInvoker.SetCompiledExpressionRootForImplementation(

                dynamicActivity, compiledExpressionRoot);
            errorMessages = null;
            return true;

        }

        #region Private Classes

        private class VRWorkflowActivityGenerateWFActivityCodeContext : IVRWorkflowActivityGenerateWFActivityCodeContext
        {
            Dictionary<string, VRWorkflowArgument> _allArguments = new Dictionary<string, VRWorkflowArgument>();
            public VRWorkflowActivityGenerateWFActivityCodeContext(VRWorkflowArgumentCollection workflowArguments)
            {
                if (workflowArguments != null)
                {
                    foreach(var arg in workflowArguments)
                    {
                        _allArguments.Add(arg.Name, arg);
                    }
                }
            }

            public List<string> OtherNameSpaceCodes { get; private set; }

            public List<string> AdditionalUsingStatements { get; private set; }

            Dictionary<string, VRWorkflowVariable> _allVariables = new Dictionary<string, VRWorkflowVariable>();

            public void AddVariables(VRWorkflowVariableCollection variables)
            {
                variables.ThrowIfNull("variables");
                foreach(var variable in variables)
                {
                    if (_allVariables.ContainsKey(variable.Name))
                        throw new Exception(String.Format("Variable '{0}' already added to the Workflow Variables", variable.Name));
                    _allVariables.Add(variable.Name, variable);
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
                if (this.OtherNameSpaceCodes == null)
                    this.OtherNameSpaceCodes = new List<string>();
                this.OtherNameSpaceCodes.Add(namespaceCode);
            }

            public void AddUsingStatement(string usingStatement)
            {
                if (this.AdditionalUsingStatements == null)
                    this.AdditionalUsingStatements = new List<string>();
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
        }


        #endregion
    }
}
