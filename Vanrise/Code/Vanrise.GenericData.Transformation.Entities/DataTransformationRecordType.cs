using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Transformation.Entities
{
    public class DataTransformationRecordType
    {
        public string RecordName { get; set; }

        public Guid? DataRecordTypeId { get; set; }

        public string FullTypeName { get; set; }

        public bool IsArray { get; set; }
    }
}
