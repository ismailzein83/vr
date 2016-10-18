using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.MainExtensions.MappingSteps
{
    public class LoadBEByIDMappingStep : Vanrise.GenericData.Transformation.Entities.MappingStep
    {
        public override Guid ConfigId { get { return new Guid("c01eeeaf-7d51-4fce-8842-5db1a8d1b39a"); } }

        public Guid BusinessEntityDefinitionId { get; set; }

        public string BusinessEntityId { get; set; }

        public string BusinessEntity { get; set; }

        public override void GenerateExecutionCode(Transformation.Entities.IDataTransformationCodeGenerationContext context)
        {
            var businessEntityManagerVariableName = context.GenerateUniqueMemberName("businessEntityManager");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new Vanrise.GenericData.Business.BusinessEntityManager();", businessEntityManagerVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.GetEntity(new Guid(\"{2}\"), {3});", BusinessEntity, businessEntityManagerVariableName, this.BusinessEntityDefinitionId, BusinessEntityId);
        }
    }
}
