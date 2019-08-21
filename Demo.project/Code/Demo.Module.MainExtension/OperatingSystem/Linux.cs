using System;
using Demo.Module.Entities;

namespace Demo.Module.MainExtension.OperatingSystem
{
    public class Linux : SoftwareOperatingSystem
    {
        public override Guid ConfigId { get { return new Guid("5621A588-D615-413C-91EF-BE405DB30BC1"); } }

        public bool HasGUI { get; set; }

        public override string GetDescription()
        {
            return HasGUI ? "Has GUI" : "No GUI";
        }
    }
}