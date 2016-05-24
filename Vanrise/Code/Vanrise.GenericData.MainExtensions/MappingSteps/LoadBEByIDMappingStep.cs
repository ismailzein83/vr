using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.MainExtensions.MappingSteps
{
    public class LoadBEByIDMappingStep : Vanrise.GenericData.Transformation.Entities.MappingStep
    {
        public int BusinessEntityDefinitionId { get; set; }

        public string BusinessEntityId { get; set; }

        public string BusinessEntity { get; set; }

        public override void GenerateExecutionCode(Transformation.Entities.IDataTransformationCodeGenerationContext context)
        {
           
        }
    }
}
