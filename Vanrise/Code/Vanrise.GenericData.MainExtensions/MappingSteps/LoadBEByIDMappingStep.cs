using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.MainExtensions.MappingSteps
{
    public class LoadBEByIDMappingStep : Vanrise.GenericData.Transformation.Entities.MappingStep
    {
        public Guid BusinessEntityDefinitionId { get; set; }

        public string BusinessEntityId { get; set; }

        public string BusinessEntity { get; set; }

        public override void GenerateExecutionCode(Transformation.Entities.IDataTransformationCodeGenerationContext context)
        {
            var businessEntityManagerVariableName = context.GenerateUniqueMemberName("businessEntityManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Business.BusinessEntityManager();", businessEntityManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.GetEntity({2}, {3});", BusinessEntity, businessEntityManagerVariableName, this.BusinessEntityDefinitionId, BusinessEntityId);
        }
    }
}
