using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRFileQuery
    {
        public string ModuleName { get; set; }
        public int UserId { get; set; }
        public Guid? ConnectionId { get; set; }
    }
}
