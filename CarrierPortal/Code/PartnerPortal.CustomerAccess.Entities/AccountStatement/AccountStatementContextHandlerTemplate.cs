using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class AccountStatementContextHandlerTemplate : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "PartnerPortal_CustomerAccess_AccountStatementContextHandler";

        public string Editor { get; set; }
    }
}
