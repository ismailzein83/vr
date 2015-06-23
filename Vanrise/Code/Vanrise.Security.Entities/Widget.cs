using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class Widget
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string directiveName { get; set; }
        public WidgetSetting Setting { get; set; }
    }
}
