using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRNamespaceItem
    {
        public Guid VRNamespaceItemId { get; set; }
        public Guid VRNamespaceId { get; set; }
        public string Name { get; set; }
        public VRNamespaceItemSettings Settings { get; set; }
        public int CreatedBy { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public DateTime CreatedTime { get; set; }
    }
    public class VRNamespaceItemSettings
    {
        public VRDynamicCodeSettings Code { get; set; }
    }
}
