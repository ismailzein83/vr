using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class Module
    {
        public Guid ModuleId { get; set; }
        
        public string Name { get; set; }
        
        public string Url { get; set; }
        public Guid? DefaultViewId { get; set; }

        public Guid? ParentId { get; set; }

        public string Icon { get; set; }
        public bool AllowDynamic { get; set; }
        public int Rank { get; set; }

        public ModuleSettings Settings { get; set; }
    }

    public class ModuleSettings
    {
        public string LocalizedName { get; set; }
    }
}
