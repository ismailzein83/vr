using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRTileSettings
    {
        public VRTileExtendedSettings ExtendedSettings { get; set; }
    }
    public abstract class VRTileExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }
    }
}
