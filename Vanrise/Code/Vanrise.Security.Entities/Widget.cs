using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class Widget
    {
        public int Id { get; set;}
        public int WidgetDefinitionId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public WidgetSetting Setting { get; set; }
    }
    public class WidgetDetails
    {
        public int Id { get; set; }
        public int WidgetDefinitionId { get; set; }
        public string WidgetDefinitionName { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string DirectiveName { get; set; }
        public WidgetDefinitionSetting WidgetDefinitionSetting { get; set; }
        public WidgetSetting Setting { get; set; }
    }
}
