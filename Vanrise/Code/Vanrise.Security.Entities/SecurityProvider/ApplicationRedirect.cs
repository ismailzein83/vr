using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class ApplicationRedirectInput
    {
        public Guid ApplicationId { get; set; }

        public string Token { get; set; }

        public string Email { get; set; }
    }

    public class ApplicationRedirectOutput
    {
        public string CookieName { get; set; }

        public AuthenticationToken AuthenticationToken { get; set; }
    }

    public class ValidateSecurityTokenInput
    {
        public Guid? ApplicationId { get; set; }

        public string Token { get; set; }
    }

    public class RegisterApplicationInput
    {
        public string ApplicationName { get; set; }

        public string ApplicationURL { get; set; }
    }

    public class RegisterApplicationOutput
    {
        public Guid ApplicationId { get; set; }
    }
}
