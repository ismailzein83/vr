
namespace Vanrise.Security.Entities
{
    public class ChangedPasswordObject
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }

    public class ChangeExpiredPasswordInput
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
    }
}
