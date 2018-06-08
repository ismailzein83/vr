using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Demo.Module.Entities
{
    public class ItemShapeConfig:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Demo_Module_ItemShape";
        public string Editor { get; set; }
    }
}
