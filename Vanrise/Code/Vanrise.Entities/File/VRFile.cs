using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRFileInfo
    {
        public long FileId { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public DateTime CreatedTime { get; set; }
    }

    public class VRFile : VRFileInfo
    {
        public byte[] Content { get; set; }

    }
}
