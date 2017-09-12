using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class MapUserToAccountInput
    {
        public string TelesDomainId { get; set; }
        public string TelesEnterpriseId { get; set; }
        public string TelesSiteId { get; set; }
        public string TelesUserId { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
        public long AccountId { get; set; }
        public Guid ActionDefinitionId { get; set; }
    }
}
