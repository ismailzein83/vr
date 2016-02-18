using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class InitializeRecordStep : MappingStep
    {
        public string RecordName { get; set; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var record = context.Records.FirstOrDefault(itm => itm.RecordName == this.RecordName);
            if (record == null)
                throw new Exception(String.Format("Record '{0}' not found", this.RecordName));
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var runtimeType = dataRecordTypeManager.GetDataRecordRuntimeType(record.DataRecordTypeId.Value);
            if (runtimeType == null)
                throw new NullReferenceException("runtimeType");
            if (!record.IsArray)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = new {1}();", this.RecordName, runtimeType.FullName);
            else
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = new List<{1}>();", this.RecordName, runtimeType.FullName);
        }
    }
}
