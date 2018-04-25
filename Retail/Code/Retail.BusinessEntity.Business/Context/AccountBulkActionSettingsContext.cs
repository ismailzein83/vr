using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountBulkActionSettingsContext : IAccountBulkActionSettingsContext
    {
        public Account Account { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsErrorOccured { get; set; }
        public AccountBulkActionSettings DefinitionSettings { get; set; }

    }
}
