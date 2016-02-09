using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordTypeSettings
    {
        public int AssemblyId { get; set; }

        public string RuntimeFQTN { get; set; }

        public int BatchAssemblyId { get; set; }

        public string BatchRuntimeFQTN { get; set; }
    }
}
