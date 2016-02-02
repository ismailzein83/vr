using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Transformation.Entities
{
    public class DataTransformationStepDefinition
    {
        public int DataTransformationStepDefinitionId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public DataTransformationStepDefinitionSettings Settings { get; set; }
    }

    public class DataTransformationStepDefinitionSettings
    {
        public string Editor { get; set; }
    }
}
