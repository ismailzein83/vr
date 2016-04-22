using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class WidgetDefinition
    {
        public int WidgetDefinitionId { get; set; }
        public string Name { get; set; }
        public WidgetDefinitionSetting Settings { get; set; }
    }
}
