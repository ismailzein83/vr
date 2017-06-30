using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public enum ProvisionStatus { Started = 0, Completed = 1 }
    public class EnterpriseAccountMappingInfo : BaseAccountExtendedSettings
    {
        public string TelesEnterpriseId { get; set; }
        public ProvisionStatus? Status { get; set; }
    }
}
