using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.AccountManager.Entities
{
    public class AccountManagerSubViewsConfigs : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_AccountManager_AccountManagerSubviewDefinition";
        public string Editor { get; set; }
    }
}
