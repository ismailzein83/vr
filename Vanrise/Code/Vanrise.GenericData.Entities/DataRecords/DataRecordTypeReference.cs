using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class DataRecordTypeReference
    {
        public int ConfigId { get; set; }

        public abstract DataRecordType GetDataRecordType(IDataRecordTypeReferenceContext context);
    }
}
