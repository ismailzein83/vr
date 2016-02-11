using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class TariffMappingStep : BaseGenericRuleMappingStep
    {
        public string  InitialRate { get; set; }
        public string DurationInSeconds { get; set; }
        public string EffectiveRate { get; set; }
        public string EffectiveDurationInSeconds { get; set; }
        public string TotalAmount { get; set; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
