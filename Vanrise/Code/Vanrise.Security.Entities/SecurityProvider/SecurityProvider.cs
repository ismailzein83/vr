using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public enum SecurityProviderAuthenticateResult { Succeeded = 0, WrongCredentials = 1, UserNotExists = 2, ActivationNeeded = 3 }
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

        public abstract SecurityProviderAuthenticateResult Authenticate(ISecurityProviderAuthenticateContext context);
    }

    public interface ISecurityProviderAuthenticateContext
    {
        SecurityProviderAuthenticationPayload Payload { get; }

        User AuthenticatedUser { set; }

        string FailureMessage { set; }
    }

    public class SecurityProviderAuthenticateContext : ISecurityProviderAuthenticateContext
    {
        public SecurityProviderAuthenticationPayload Payload { get; set; }

        public User AuthenticatedUser { get; set; }

        public string FailureMessage { get; set; }
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
