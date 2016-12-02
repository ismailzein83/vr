using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.Voice.MainExtensions.TransformationSteps
{
    public class PriceVoiceEventStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("FB2D7DC4-AF79-4068-8452-1058AF7544D7"); } }

        public string AccountId { get; set; }
        public string ServiceTypeId { get; set; }
        public string RawCDR { get; set; }
        public string MappedCDR { get; set; }
        public string Duration { get; set; }
        public string EventTime { get; set; }


        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
