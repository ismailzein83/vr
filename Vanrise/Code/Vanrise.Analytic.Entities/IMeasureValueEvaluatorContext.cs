using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public interface IMeasureValueEvaluatorContext
    {
        Dictionary<string, MeasureConfiguration> Measures { get; }
        string GenerateUniqueMemberName(string memberName);
        void AddGlobalMember(string memberDeclarationCode);
        void AddCodeToCurrentInstanceExecutionBlock(string codeLineTemplate, params object[] placeholders);

    }
}
