using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRFile : VRFileInfo
    {
        public byte[] Content { get; set; }
    }

    public class VRFileInfo
    {
        public long FileId { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ModuleName { get; set; }
        public int? UserId { get; set; }
        public bool IsTemp { get; set; }
        public VRFileSettings Settings { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid? FileUniqueId { get; set; }
    } 

    public class VRFileSettings
    {
        public VRFileExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class VRFileExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract bool DoesUserHaveViewAccess(IVRFileDoesUserHaveViewAccessContext context);
    }

    public interface IVRFileDoesUserHaveViewAccessContext
    {
        int UserId { get; }
    }

    
}
