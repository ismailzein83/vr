using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class PasswordValidationInfo
    {
        public string RequirementsMessage { get; set; }

        public bool RequiredPassword { get; set; }
    }
}
