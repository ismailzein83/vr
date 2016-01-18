using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class WidgetDetail
    {
        public Widget Entity { get; set; }
        public string WidgetDefinitionName { get; set; }
        public string DirectiveName { get; set; }
        public WidgetDefinitionSetting WidgetDefinitionSetting { get; set; }
    }
}
