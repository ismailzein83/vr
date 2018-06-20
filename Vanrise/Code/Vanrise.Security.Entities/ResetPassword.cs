using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class ResetPasswordInput
    {
        public int UserId { get; set; }
        public string Password { get; set; }
    }

    public class EmailResetPasswordInput
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ForgotPasswordInput
    {
        public string Email { get; set; }
    }

    public class ActivatePasswordInput
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string TempPassword { get; set; }
    }
}
