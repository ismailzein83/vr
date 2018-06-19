using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Security.Entities
{
    public class Constants
    {
        public const string ResetPasswordType = "VR_Security_ResetPassword";
        public const string ForgotPasswordType = "VR_Security_ForgotPassword";
        public const string NewPasswordType = "VR_Security_NewPassword";
    }
    public class SecurityProviderConfigs : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Sec_SecurityProviderSettings";
        public string Editor { get; set; }
    }



}
