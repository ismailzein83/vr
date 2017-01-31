using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountActionDefinitionInfo
    {
        public Guid AccountActionDefinitionId { get; set; }
        public string Name { get; set; }
        public string BackendExecutorSettingEditor { get; set; }
    }
}
