using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using Vanrise.Common;
using System;
using System.Text;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Business;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;

namespace Vanrise.BusinessProcess.Business

{
    public class CustomCodeTaskManager
    {
        #region public methods

        public Type GetCachedCustomCodeTaskTypeByTaskId(Guid taskId)
        {
            SchedulerTask task = new SchedulerTaskManager().GetTask(taskId);
            WFTaskActionArgument wfTaskActionArgument = task.TaskSettings.TaskActionArgument.CastWithValidate<WFTaskActionArgument>("wfTaskActionArgument");
            CustomCodeBPArgument customCodeBPArgument = wfTaskActionArgument.ProcessInputArguments.CastWithValidate<CustomCodeBPArgument>("customCodeBPArgument");
            string cacheName = string.Format("CustomCodeTaskManager_GetCachedGetCustomCodeTaskType_{0}", taskId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SchedulerTaskManager.CacheManager>().GetOrCreateObject(cacheName,() =>{
                    Type runtimeType;
                    TryCompileCustomCodeTask(customCodeBPArgument.TaskCode, customCodeBPArgument.ClassDefinitions, out runtimeType);
                    return runtimeType;
             });
        }
        public CustomCodeTaskCompilationOutput TryCompileCustomCodeTask(string taskCode, string classDefinitions)
        {
            Type runtimeType;
            return TryCompileCustomCodeTask(taskCode, classDefinitions, out runtimeType);
        }
        public CustomCodeTaskCompilationOutput TryCompileCustomCodeTask(string taskCode, string classDefinitions, out Type runtimeType)
        {
            string fullTypeName;
            string customCodeClass = BuildCustomCodeClassDefinition(out fullTypeName, taskCode, classDefinitions);
            CustomCodeTaskCompilationOutput output = new CustomCodeTaskCompilationOutput();
            CSharpCompilationOutput compilationOutput;
            runtimeType = null;
            if (!CSharpCompiler.TryCompileClass(customCodeClass, out compilationOutput))
            {
                output.ErrorMessages = compilationOutput.ErrorMessages;
                output.Result = false;
            }
            else
            {
                output.ErrorMessages = null;
                output.Result = true;
                runtimeType = compilationOutput.OutputAssembly.GetType(fullTypeName);
            }         
            return output;
        }


        #endregion

        #region private methods

        string BuildCustomCodeClassDefinition(out string fullTypeName, string taskCode, string classDefinitions)
        {

            string className = "CustomCodeHandlerExecutor";
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;
                using System.Text;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;
                using System.Linq;
                using Vanrise.Common;
                using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.UCT;
                using Vanrise.Common.Excel;

                namespace #NAMESPACE#
                {
                    #CLASSDEFINITIONS#

                    public class  #CLASSNAME# : Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.ICustomCodeHandler
                    { 
                        public void Execute(Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments.ICustomCodeExecutionContext context)
                        {
                            #TASKCODE#
                        }
                    }
                }");

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.BusinessProcess");
            classDefinitionBuilder.Replace("#CLASSDEFINITIONS#", classDefinitions);
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            classDefinitionBuilder.Replace("#TASKCODE#", taskCode);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);
            return classDefinitionBuilder.ToString();
        }

        #endregion
    }
}