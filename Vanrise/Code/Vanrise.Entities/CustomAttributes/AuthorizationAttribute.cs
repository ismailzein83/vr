using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class AuthorizationAttribute : Attribute
    {
        public string Permissions { get; set; }
    }
}
