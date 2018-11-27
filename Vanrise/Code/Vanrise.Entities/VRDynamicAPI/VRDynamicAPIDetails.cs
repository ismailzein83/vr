using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDynamicAPIDetails
    {
        public long VRDynamicAPIId { get; set; }
        public string Name { get; set; }
        public string ModuleName { get; set; }
        public VRDynamicAPISettings Settings { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedByDescription { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string LastModifiedByDescription { get; set; }
        public string APIDescription { get; set; }
    }
}
