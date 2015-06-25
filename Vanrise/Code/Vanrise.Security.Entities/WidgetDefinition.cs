using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class WidgetDefinition
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string directiveName { get; set; }
        public WidgetDefinitionSetting Setting { get; set; }
    }
}
