using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.Entities
{
    public enum AuthenticationResult {  Succeeded = 0, Failed = -1, AccountDisabled = -2}
    public class AuthenticationOutput
    {
        public AuthenticationResult Result { get; set; }
        
        public User User { get; set; }
    }
}
