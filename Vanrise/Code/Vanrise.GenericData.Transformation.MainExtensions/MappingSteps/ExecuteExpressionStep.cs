using System;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.MainExtensions.MappingSteps
{
    public class ExecuteExpressionStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("CE33AAB3-2C0A-4F8C-8FBD-FF6B91677C8F"); } }

        public string Expression { get; set; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            if (!string.IsNullOrEmpty(Expression))
                context.AddCodeToCurrentInstanceExecutionBlock("{0};", this.Expression);
        }
    }
}
