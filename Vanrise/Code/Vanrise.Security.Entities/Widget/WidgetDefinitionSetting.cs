using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public enum Section { Summary = 0, Body = 1 }
    public class WidgetDefinitionSetting
    {
        public string DirectiveTemplateURL { get; set; }
        public List<Section> Sections { get; set; }
    }
}
