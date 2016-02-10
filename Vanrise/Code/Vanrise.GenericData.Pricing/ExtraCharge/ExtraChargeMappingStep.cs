using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Pricing.ExtraCharge
{
    public class ExtraChargeMappingStep : BaseGenericRuleMappingStep
    {
        public string InitialRate { get; set; }
        public string EffectiveRate { get; set; }
        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
