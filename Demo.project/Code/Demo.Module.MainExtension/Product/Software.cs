using Demo.Module.Entities;
using System;
using System.Collections.Generic;

namespace Demo.Module.MainExtension.Product
{
    public class Software : ProductSettings
    {
        public override Guid ConfigId { get { return new Guid("725A7CB7-0891-44F7-ADFC-09D956090602"); } }
        public double Size { get; set; }
        public string Language { get; set; }
        public List<SoftwareOperatingSystem> SoftwareOperatingSystems { get; set; }

        public override string GetDescription()
        {
            return $"Software of size: { Size } GB written using { Language } works on " + 
                SoftwareOperatingSystems != null ? SoftwareOperatingSystems.Count.ToString() : "0" + $" operating systems - costs: { Price.ToString() }$";
        }
    }
}