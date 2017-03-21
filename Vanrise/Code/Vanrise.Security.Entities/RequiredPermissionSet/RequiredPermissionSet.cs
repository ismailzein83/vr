using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class RequiredPermissionSet
    {
        public int RequiredPermissionSetId { get; set; }

        public string Module { get; set; }

        public string RequiredPermissionString { get; set; }
    }
}
