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
        public Guid DataTransformationDefinitionId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public List<DataTransformationRecordType> RecordTypes { get; set; }

        public List<MappingStep> MappingSteps { get; set; }
    }
}
