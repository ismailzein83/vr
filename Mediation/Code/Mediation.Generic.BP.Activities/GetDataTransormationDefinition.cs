using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.GenericData.Transformation;
using Vanrise.GenericData.Transformation.Entities;

namespace Mediation.Generic.BP.Activities
{
    public sealed class GetDataTransormationDefinition : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> TransformationDefinitionId { get; set; }
        [RequiredArgument]
        public InOutArgument<DataTransformationDefinition> DataTransformationDefinition { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            Guid trasformationDefinitionId = TransformationDefinitionId.Get(context);
            DataTransformationDefinitionManager dataTransformationManager = new DataTransformationDefinitionManager();
            DataTransformationDefinition transformationDefinition = dataTransformationManager.GetDataTransformationDefinition(trasformationDefinitionId);
            if (transformationDefinition == null)
                throw new NullReferenceException(string.Format("GetDataTransormationDefinition: TransformationDefinitionId {0}", trasformationDefinitionId));
            DataTransformationDefinition.Set(context, transformationDefinition);
        }
    }
}
