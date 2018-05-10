using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class SecurityProvider
    {
        public Guid SecurityProviderId { get; set; }

        public string Name { get; set; }

        public Guid DefinitionId { get; set; }

        public SecurityProviderSettings Settings { get; set; }
    }

    public class SecurityProviderSettings
    {
        public SecurityProviderExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class SecurityProviderExtendedSettings
    {
        public virtual string FindUserEditor { get; set; }

        public virtual string AuthenticateUserEditor { get; set; }

        public abstract bool Authenticate(ISecurityProviderAuthenticateContext context);
    }

    public interface ISecurityProviderAuthenticateContext
    {
        SecurityProviderAuthenticationPayload Payload { get; }

        User AuthenticatedUser { set; }

        string FailureMessage { get; }
    }

    public abstract class SecurityProviderAuthenticationPayload
    {

    }

    public class EmailPasswordSecurityProviderAuthenticationPayload : SecurityProviderAuthenticationPayload
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
