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
            get { throw new NotImplementedException(); }
        }
        public override string RuntimeEditor { get { throw new NotImplementedException(); } }
        public Guid AccountManagerAssignementDefinitionId { get; set; }
    }
}
