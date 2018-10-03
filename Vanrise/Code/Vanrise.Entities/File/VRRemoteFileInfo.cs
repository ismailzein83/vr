using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRRemoteFile : VRRemoteFileInfo
    {
        public byte[] Content { get; set; }
    }
    public class VRRemoteFileInfo
    {
        public long FileId { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ModuleName { get; set; }
        public int? UserId { get; set; }
        public bool IsTemp { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid? FileUniqueId { get; set; }
    }
}
