using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class CloneDataRecordStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("FB8AB50E-77EB-40DD-B246-40E3B4E23539"); } }

        public string SourceRecordName { get; set; }

        public string TargetRecordName { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var sourceRecord = context.Records.FirstOrDefault(itm => itm.RecordName == this.SourceRecordName);
            if (sourceRecord == null)
                throw new Exception(String.Format("Record '{0}' not found", this.SourceRecordName));
            
            if (!sourceRecord.DataRecordTypeId.HasValue)
                throw new NullReferenceException(String.Format("DataRecordTypeId: '{0}'", this.SourceRecordName));

            var targetRecord = context.Records.FirstOrDefault(itm => itm.RecordName == this.TargetRecordName);
            if (targetRecord == null)
                throw new Exception(String.Format("Record '{0}' not found", this.TargetRecordName));
            
            if (!targetRecord.DataRecordTypeId.HasValue)
                throw new NullReferenceException(String.Format("DataRecordTypeId: '{0}'", this.TargetRecordName));

            if (sourceRecord.DataRecordTypeId.Value != targetRecord.DataRecordTypeId.Value)
                throw new Exception(String.Format("'{0}' and '{1}' are of different dataRecordType", this.TargetRecordName));
         
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var runtimeType = dataRecordTypeManager.GetDataRecordRuntimeType(targetRecord.DataRecordTypeId.Value);
            if (runtimeType == null)
                throw new NullReferenceException("runtimeType");
            
            string fullTypeName = CSharpCompiler.TypeToString(runtimeType);

            Dictionary<string, DataRecordField> dataRecordTypeFields = dataRecordTypeManager.GetDataRecordTypeFields(sourceRecord.DataRecordTypeId.Value);
            if (dataRecordTypeFields == null)
                throw new NullReferenceException(String.Format("DataRecordTypeFields of dataRecordTypeId: '{0}'", sourceRecord.DataRecordTypeId.Value));

            context.AddCodeToCurrentInstanceExecutionBlock("{0} = new {1}();", this.TargetRecordName, fullTypeName);

            foreach (var itm in dataRecordTypeFields.Keys)
            {
                context.AddCodeToCurrentInstanceExecutionBlock("{0}.{1} = {2}.{1};", this.TargetRecordName, itm, this.SourceRecordName);
            }
        }
    }
}