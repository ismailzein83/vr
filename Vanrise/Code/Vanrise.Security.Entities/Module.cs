using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class Module
    {
        public int ModuleId { get; set; }
        
        public string Name { get; set; }

        public string Title { get; set; }
        
        public string Url { get; set; }

        public int ParentId { get; set; }

        public string Icon { get; set; }
        public bool AllowDynamic { get; set; }
    }
}
