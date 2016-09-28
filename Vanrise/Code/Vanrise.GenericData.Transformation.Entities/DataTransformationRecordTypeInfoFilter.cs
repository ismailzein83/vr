using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Transformation.Entities
{
    public class DataTransformationRecordTypeInfoFilter
    {
        public List<Guid> DataRecordTypeIds { get; set; }
        public bool? IsArray { get; set; }
    }
}
