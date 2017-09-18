using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum ColumnWidthEnum { QuarterRow = 0, FullRow = 1, HalfRow = 2, OneThirdRow = 3, TwoThirdRow=4 }
    public class VRTileSettings
    {
        public ColumnWidthEnum NumberOfColumns { get; set; }
        public VRTileExtendedSettings ExtendedSettings { get; set; }
    }
    public abstract class VRTileExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeEditor { get; }
    }
}
