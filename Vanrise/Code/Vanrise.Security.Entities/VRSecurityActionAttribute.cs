using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class VRSecurityActionAttribute : Attribute
    {
        public VRSecurityActionAttribute(string actionName)
        {
            this.ActionName = actionName;
        }
        public string ActionName { get; set; }
    }
}
