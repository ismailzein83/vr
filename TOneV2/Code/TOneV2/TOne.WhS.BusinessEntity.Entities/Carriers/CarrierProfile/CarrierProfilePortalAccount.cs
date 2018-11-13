using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum PortalCarrierAccountType { All = 0, Specific = 1 }
    public class CarrierProfilePortalAccount
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int TenantId { get; set; }
        public PortalCarrierAccountType Type { get; set; }
        public UserStatus UserStatus { get; set; }
        public List<PortalCarrierAccount> CarrierAccounts { get; set; }
    }
    public class PortalCarrierAccount
    {
        public int CarrierAccountId { get; set; }
    }

    public class CarrierProfilePortalAccountDetail
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int TenantId { get; set; }
        public UserStatus UserStatus { get; set; }
        public string UserStatusDescription { get; set; }
    }

    public class PortalAccountSettings
    {
        public List<CarrierProfilePortalAccount> CarrierProfilePortalAccounts { get; set; }
    }
    public class CarrierProfilePortalAccountSettings
    {
        public List<PortalCarrierAccount> PortalCarrierAccounts { get; set; }
        public int CarrierProfileId { get; set; }
        public PortalAccountSettings PortalAccountSettings { get; set; }
    }
}
