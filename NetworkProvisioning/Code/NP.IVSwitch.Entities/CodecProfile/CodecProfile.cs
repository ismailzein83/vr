using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class CodecProfile
    {
        public int CodecProfileId { get; set; }

        public string ProfileName { get; set; }

        public DateTime CreateDate { get; set; }

        public List<int> CodecDefId { get; set; }
    }
}
