using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Transformation.Entities
{
    public class DataTransformationStepConfig
    {
        public int DataTransformationStepConfigId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Editor { get; set; }

        public string StepPreviewUIControl { get; set; }
    }
}
