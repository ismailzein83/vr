using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public class DataTransformationStepConfig:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_DataTransformationStepConfig";

        public string Editor { get; set; }

        public string StepPreviewUIControl { get; set; }
    }
}
