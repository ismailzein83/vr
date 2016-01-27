using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class AssignFieldStep : MappingStep
    {
        public string SourceRecordName { get; set; }
        public string SourceFieldName { get; set; }

        public string TargetRecordName { get; set; }

        public string TargetFieldName { get; set; }
        

        public override void Execute(IMappingStepExecutionContext context)
        {
            DataRecord sourceRecord = context.GetDataRecord(this.SourceRecordName);
            DataRecord targetRecord = context.GetDataRecord(this.TargetRecordName);
            targetRecord[this.TargetFieldName] = sourceRecord[this.SourceFieldName];
        }
    }
}
