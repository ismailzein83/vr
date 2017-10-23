using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.APIEntities
{
    public class ClientChildAccountInfo
    {
        public long AccountId { get; set; }
        public string Name { get; set; }
        public Guid TypeId { get; set; }
    }
}
