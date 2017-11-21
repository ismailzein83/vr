using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Entities;

namespace Retail.BusinessEntity.Business
{
    public class RetailAccountAccountManagerSubViewDefinition : AccountManagerSubViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("C16E8C1C-0072-4B80-832A-753F16E1054B"); }
        }
        public override string RuntimeEditor { get { return "retail-be-accountmanager-accountsubviewruntime-search"; } }
        public Guid AccountManagerAssignementDefinitionId { get; set; }
    }
}
