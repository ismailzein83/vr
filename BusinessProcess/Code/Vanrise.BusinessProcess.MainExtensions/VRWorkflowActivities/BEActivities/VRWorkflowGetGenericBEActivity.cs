using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities
{
    public class VRWorkflowGetGenericBEActivity : VRWorkflowGetBEActivitySettings
    {
        public List<VRWorkflowGetGenericBEActivityOutputItem> OutputItems { get; set; }
        public override string GenerateCode(IVRWorkflowGetBEActivitySettingsGenerateCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("var genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();");
            codeBuilder.AppendLine($@"var genericBusinessEntity = genericBEManager.GetGenericBusinessEntity({context.EntityIdCode}, new Guid(""{context.EntityDefinitionId}""));");
            codeBuilder.AppendLine("genericBusinessEntity.ThrowIfNull(\"genericBusinessEntity\");");
            codeBuilder.AppendLine("genericBusinessEntity.FieldValues.ThrowIfNull(\"genericBusinessEntity.FieldValues\");");
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var dataRecordTypeId = new GenericBusinessEntityDefinitionManager().GetGenericBEDataRecordTypeId(context.EntityDefinitionId);
            var stringTypeOfdataRecordType  = dataRecordTypeManager.GetDataRecordRuntimeTypeAsString(dataRecordTypeId);


            codeBuilder.AppendLine($@"Dictionary<string, dynamic> fieldValues = new Dictionary<string, dynamic>();");
            codeBuilder.AppendLine($@"foreach(var fieldValue in genericBusinessEntity.FieldValues)");
            codeBuilder.AppendLine("{");
            codeBuilder.AppendLine("fieldValues.Add(fieldValue.Key,fieldValue.Value);");
            codeBuilder.AppendLine("}");

            codeBuilder.AppendLine($@"var dataRecordObject = new {stringTypeOfdataRecordType}(fieldValues);");
            if (OutputItems != null && OutputItems.Count > 0)
            {
                foreach (var outputItem in OutputItems)
                {
                    if (outputItem.Value != null)
                    {
                        codeBuilder.AppendLine($@"{outputItem.Value.GetCode(null)} = dataRecordObject.{outputItem.FieldName};");
                    }
                }
            }
            return codeBuilder.ToString();
        }
        public class VRWorkflowGetGenericBEActivityOutputItem
        {
            public string FieldName { get; set; }

            public VRWorkflowExpression Value { get; set; }
        }
    }
}
