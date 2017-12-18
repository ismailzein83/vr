using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRExclusiveSessionQuery
    {
        public Guid SessionTypeId { get; set; }
        public string TargetName { get; set; }
    }
}
