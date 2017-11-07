using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.AccountManager.Entities
{
    public class AccountManagerAssignmentConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_AccountManager_AccountManagerAssignmentDefinition";
        public string Editor { get; set; }
    }
}
