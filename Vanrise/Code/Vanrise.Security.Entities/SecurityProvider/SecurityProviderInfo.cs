using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class SecurityProviderInfo
    {
        public Guid SecurityProviderId { get; set; }

        public string Name { get; set; }

        public string AuthenticateUserEditor { get; set; }

        public string FindUserEditor { get; set; }

        public bool SupportPasswordManagement { get; set; }
    }

    public class SecurityProviderFilter
    {
        public List<ISecurityProviderFilter> Filters { get; set; }
    }

    public interface ISecurityProviderFilter
    {
        bool IsExcluded(ISecurityProviderFilterContext context);
    }

    public interface ISecurityProviderFilterContext
    {
        SecurityProvider SecurityProvider { get; set; }
    }

    public class SecurityProviderFilterContext : ISecurityProviderFilterContext
    {
        public SecurityProvider SecurityProvider { get; set; }
    }
}