using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS.Plugins.Framework
{
    public class PluginResource
    {
        public string Title { get; set; }
        //public string FullyQualifiedTypeName { get; set; }
        public string ResourceName { get; set; }
        public bool ShouldAppearInMenu { get; set; }
        public byte[] Contents { get; set; }
    }
}
