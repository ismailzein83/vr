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
    public class InitializeRecordStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("f02ac0b5-79a4-4343-8081-85cd3787b88c"); } }

        public string RecordName { get; set; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            var record = context.Records.FirstOrDefault(itm => itm.RecordName == this.RecordName);
            if (record == null)
                throw new Exception(String.Format("Record '{0}' not found", this.RecordName));
            string fullTypeName;
            if (record.DataRecordTypeId.HasValue)
            {
                DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                var runtimeType = dataRecordTypeManager.GetDataRecordRuntimeType(record.DataRecordTypeId.Value);
                if (runtimeType == null)
                    throw new NullReferenceException("runtimeType");
                fullTypeName = CSharpCompiler.TypeToString(runtimeType);
            }
            else
                fullTypeName = record.FullTypeName;
            if (!record.IsArray)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = new {1}();", this.RecordName, fullTypeName);
            else
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = new List<dynamic>();", this.RecordName);
        }
    }
}
