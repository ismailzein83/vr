using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Demo.BestPractices.Entities
{
    public class ChildShapeConfig:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Demo_BestPractices_ChildShape";
        public string Editor { get; set; }
    }
}
