using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public enum InvalidAccess { None = 0, TokenExpired = 1, UnauthorizeAccess = 2, LicenseExpired = 3 }
    public class AuthenticationToken
    {
        public string TokenName { get; set; }

        public string UserName { get; set; }

        public Guid SecurityProviderId { get; set; }

        public bool SupportPasswordManagement { get; set; }

        public string UserDisplayName { get; set; }

        public int ExpirationIntervalInMinutes { get; set; }

        public int ExpirationIntervalInSeconds { get; set; }

        public string Token { get; set; }

        public long? PhotoFileId { get; set; }

        public int? PasswordExpirationDaysLeft { get; set; }
    }

    public class SecurityToken
    {
        public int UserId { get; set; }

        public DateTime IssuedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public List<SecurityTokenCloudApplication> AccessibleCloudApplications { get; set; }
    }

    public class SecurityTokenCloudApplication
    {
        public int ApplicationId { get; set; }
    }

    public abstract class SecurityTokenExtensionBehavior
    {
        public abstract void AddExtensionsToToken(ISecurityTokenExtensionContext context);
    }

    public interface ISecurityTokenExtensionContext
    {
        SecurityToken Token { get; }
    }
}
