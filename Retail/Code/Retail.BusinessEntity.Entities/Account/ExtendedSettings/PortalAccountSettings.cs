using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class PortalAccountSettings : BaseAccountExtendedSettings
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int TenantId { get; set; }
        public List<AdditionalPortalAccountSettings> AdditionalUsers { get; set; }
    }
    public class AdditionalPortalAccountSettings 
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int TenantId { get; set; }
    }
    public class PortalAccountDetail
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int TenantId { get; set; }
        public UserStatus UserStatus { get; set; }
        public string UserStatusDescription { get; set; }
    }
}
