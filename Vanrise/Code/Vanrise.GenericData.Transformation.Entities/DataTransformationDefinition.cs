using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public class DataTransformationDefinition
    {
        public int DataTransformationDefinitionId { get; set; }

        public Dictionary<string, DataRecordTypeReference> RecordTypes { get; set; }

        public List<MappingStep> MappingSteps { get; set; }
    }
}
