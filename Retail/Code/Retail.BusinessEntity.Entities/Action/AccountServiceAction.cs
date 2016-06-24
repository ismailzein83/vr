using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ServiceAction : BaseAction
    {
        public long AccountId { get; set; }

        public int ServiceTypeId { get; set; }
    }
}
