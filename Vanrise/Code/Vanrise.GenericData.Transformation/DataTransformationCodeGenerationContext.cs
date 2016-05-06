﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation
{
    internal class DataTransformationCodeGenerationContext : IDataTransformationCodeGenerationContext
    {
        DataTransformationDefinition _dataTransformationDefinition;
        public DataTransformationCodeGenerationContext(DataTransformationDefinition dataTransformationDefinition)
        {
            _dataTransformationDefinition = dataTransformationDefinition;
            
        }

        StringBuilder _globalMembersBuilder;
        StringBuilder _instanceExecutionBlockBuilder;

        #region IDataTransformationCodeGenerationContext

        string IDataTransformationCodeGenerationContext.GenerateUniqueMemberName(string memberName)
        {
            return String.Format("{0}_{1}", memberName, Guid.NewGuid().ToString("N"));
        }

        void IDataTransformationCodeGenerationContext.AddGlobalMember(string memberDeclarationCode)
        {
            _globalMembersBuilder.AppendLine(memberDeclarationCode);
            _globalMembersBuilder.AppendLine();
        }

        void IDataTransformationCodeGenerationContext.AddCodeToCurrentInstanceExecutionBlock(string codeLineTemplate, params object[] placeholders)
        {
            if (placeholders != null && placeholders.Length>0)
             _instanceExecutionBlockBuilder.AppendFormat(codeLineTemplate, placeholders);
            else
             _instanceExecutionBlockBuilder.Append(codeLineTemplate);
            _instanceExecutionBlockBuilder.AppendLine();
        }

        void IDataTransformationCodeGenerationContext.GenerateStepsCode(IEnumerable<MappingStep> steps)
        {
            foreach (var step in steps)
            {
                step.GenerateExecutionCode(this);
            }
        }

        List<DataTransformationRecordType> IDataTransformationCodeGenerationContext.Records
        {
            get
            {
                return this._dataTransformationDefinition.RecordTypes;
            }
        }

        #endregion

        public bool TryBuildRuntimeType(out DataTransformationRuntimeType runtimeType, out List<string> errorMessages)
        {
            _globalMembersBuilder = new StringBuilder();
            _instanceExecutionBlockBuilder = new StringBuilder();
            (this as IDataTransformationCodeGenerationContext).GenerateStepsCode(_dataTransformationDefinition.MappingSteps);
            string fullTypeName;
            string classDefinition = BuildClassDefinition(out fullTypeName);

            CSharpCompilationOutput compilationOutput;
            if(!CSharpCompiler.TryCompileClass(classDefinition, out compilationOutput))
            {
                runtimeType = null;
                errorMessages = compilationOutput.ErrorMessages;                
                return false;               
            }
            var executorType = compilationOutput.OutputAssembly.GetType(fullTypeName);
            if (executorType == null)
                throw new NullReferenceException("executorType");
            runtimeType = new DataTransformationRuntimeType
            {
                ExecutorType = executorType
            };
            errorMessages = null;
            return true;
        }

       

        private string BuildClassDefinition(out string fullTypeName)
        {           
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            foreach (var recordType in _dataTransformationDefinition.RecordTypes)
            {
                string dataRecordRuntimeType;

                if (recordType.DataRecordTypeId.HasValue)
                {
                    dataRecordRuntimeType = CSharpCompiler.TypeToString(dataRecordTypeManager.GetDataRecordRuntimeType(recordType.DataRecordTypeId.Value));
                    if (recordType.IsArray)
                        (this as IDataTransformationCodeGenerationContext).AddGlobalMember(String.Format("public List<dynamic> {0} = new List<dynamic>();", recordType.RecordName));
                    else
                        (this as IDataTransformationCodeGenerationContext).AddGlobalMember(String.Format("public {0} {1} = new {0}();", dataRecordRuntimeType, recordType.RecordName));
                }

                else
                {
                    dataRecordRuntimeType = "dynamic";
                    (this as IDataTransformationCodeGenerationContext).AddGlobalMember(String.Format("public {0} {1};", dataRecordRuntimeType, recordType.RecordName));
                }
                    
               
            }
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;

                namespace #NAMESPACE#
                {
                    public class #CLASSNAME# : #EXECUTORBASE#
                    {                        
                        #GLOBALMEMBERS#

                        void #EXECUTORBASE#.Execute()
                        {
                            #EXECUTIONCODE#
                        }
                    }
                }
                ");

            classDefinitionBuilder.Replace("#EXECUTORBASE#", typeof(IDataTransformationExecutor).FullName);
            classDefinitionBuilder.Replace("#GLOBALMEMBERS#", _globalMembersBuilder.ToString());
            classDefinitionBuilder.Replace("#EXECUTIONCODE#", _instanceExecutionBlockBuilder.ToString());
            
            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.GenericData.Transformation.Runtime");
            string className = "DataTransformationExecutor";
            classDefinitionBuilder.Replace("#NAMESPACE#", classNamespace);
            classDefinitionBuilder.Replace("#CLASSNAME#", className);
            fullTypeName = String.Format("{0}.{1}", classNamespace, className);
            
            return classDefinitionBuilder.ToString();
        }
    }
}
