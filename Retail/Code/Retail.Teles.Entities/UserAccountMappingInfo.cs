using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class UserAccountMappingInfo : BaseAccountExtendedSettings
    {
        public string TelesDomainId { get; set; }
        public string TelesEnterpriseId { get; set; }
        public string TelesSiteId { get; set; }
        public string TelesUserId { get; set; }
        public ProvisionStatus? Status { get; set; }
    }
}
