using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.AccountBalance.Entities
{
    public class FinancialAccountTypeConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_AccountBalance_FinancialAccountType";

        public string DefinitionEditor { get; set; }
    }
}
