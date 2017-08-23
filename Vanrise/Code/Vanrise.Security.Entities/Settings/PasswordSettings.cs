using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class PasswordSettings
    {
        public int PasswordLength { get; set; }

        public PasswordComplexity? PasswordComplexity { get; set; }
    }

    public enum PasswordComplexity
    {

        Medium = 1,
        Strong = 2
    }
}
