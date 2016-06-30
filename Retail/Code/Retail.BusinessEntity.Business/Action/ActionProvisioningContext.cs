using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class ActionProvisioningContext : IActionProvisioningContext
    {
        public EntityType EntityType { get; set; }

        public long EntityId { get; set; }

        public ActionProvisioningOutput ExecutionOutput
        {
            get;
            set;
        }


        public bool IsWaitingResponse
        {
            get;
            set;
        }
    }
}
