using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class ForLoopStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("d7ce9698-2721-467e-9820-1ed44b446d0d"); } }

        public List<MappingStep> Steps { get; set; }

        public string ArrayVariableName { get; set; }

        public string IterationVariableName { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            string variableType = "var";
            var arrayRecord = context.Records.FirstOrDefault(itm => itm.RecordName == this.ArrayVariableName);
            if (arrayRecord != null)
            {
                if (arrayRecord.DataRecordTypeId.HasValue)
                {
                    DataRecordTypeManager recordTypeManager = new DataRecordTypeManager();
                    var arrayRuntimeType = recordTypeManager.GetDataRecordRuntimeType(arrayRecord.DataRecordTypeId.Value);
                    if (arrayRuntimeType != null)
                        variableType = CSharpCompiler.TypeToString(arrayRuntimeType);
                }
                else if (!String.IsNullOrEmpty(arrayRecord.FullTypeName))
                    variableType = arrayRecord.FullTypeName;
            }
            context.AddCodeToCurrentInstanceExecutionBlock("foreach({0} {1} in {2})", variableType, this.IterationVariableName, this.ArrayVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{");
            context.GenerateStepsCode(this.Steps);
            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
