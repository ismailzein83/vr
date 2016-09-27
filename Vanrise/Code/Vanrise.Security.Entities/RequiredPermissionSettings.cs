using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class RequiredPermissionSettings
    {
        public List<RequiredPermissionEntry> Entries { get; set; }
    }

    public class RequiredPermissionEntry
    {
        public Guid EntityId { get; set; }

        public List<string> PermissionOptions { get; set; }
    }
}
