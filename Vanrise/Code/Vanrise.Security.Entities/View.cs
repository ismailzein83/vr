using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class View
    {
        public int ViewId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public int ModuleId { get; set; }

        public Dictionary<string, List<string>> RequiredPermissions { get; set; }
    }
}
