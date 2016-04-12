using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class CDRSourceProfile
    {
        public int CDRSourceProfileId { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public CDRSourceProfileSettings Settings { get; set; }
    }
}
