using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum AuthenticateOperationResult { Succeeded = 0, Failed = 1, Inactive = 2, WrongCredentials = 3, UserNotExists = 4 }

    public class AuthenticateOperationOutput<T>
    {
        public AuthenticateOperationResult Result { get; set; }

        public string Message { get; set; }

        public T AuthenticationObject { get; set; }
    }
}
