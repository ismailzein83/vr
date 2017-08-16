using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class AccountStatementExtendedSettingsConfigs : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "PartnerPortal_CustomerAccess_AccountStatementExtendedSettings";
        public string Editor { get; set; }
    }
}
