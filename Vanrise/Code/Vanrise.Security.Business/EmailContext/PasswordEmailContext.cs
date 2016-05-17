using Vanrise.Entities;

namespace Vanrise.Security.Business
{
    public class PasswordEmailContext : IEmailContext
    {
        public string Name { get; set; }

        public string Password { get; set; }

    }
}