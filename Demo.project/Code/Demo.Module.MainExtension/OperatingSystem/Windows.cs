using System;
using Demo.Module.Entities;

namespace Demo.Module.MainExtension.OperatingSystem
{
    public class Windows : SoftwareOperatingSystem
    {
        public override Guid ConfigId { get { return new Guid("FEAED651-AD56-49A8-A699-90352965522F"); } }

        public string LicenseKey { get; set; }

        public override string GetDescription()
        {
            return "Licence key: " + LicenseKey;
        }
    }
}