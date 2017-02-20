using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountGridDefinition
    {
        public List<AccountGridColumnDefinition> ColumnDefinitions { get; set; }

        public List<AccountGridExportColumnDefinition> ExportColumnDefinitions { get; set; }
    }

    public class AccountGridColumnDefinition
    {
        public string FieldName { get; set; }

        public string Header { get; set; }

        public bool IsAvailableInRoot { get; set; }

        public bool IsAvailableInSubAccounts { get; set; }

        public AccountCondition SubAccountsAvailabilityCondition { get; set; }
    }

    public class AccountGridExportColumnDefinition
    {
        public string FieldName { get; set; }

        public string Header { get; set; }
    }
}
