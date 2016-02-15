using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public interface IDataTransformationCodeGenerationContext
    {
        string GenerateUniqueMemberName(string memberName);

        void AddGlobalMember(string memberDeclarationCode);

        //void AddCodeToDefinitionExecutionBlock(string codeLineTemplate, params object[] placeholders);

        void AddCodeToCurrentInstanceExecutionBlock(string codeLineTemplate, params object[] placeholders);

        void GenerateStepsCode(IEnumerable<MappingStep> steps);
    }
}
