using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountTypeExtendedSettingsConfig:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_AccountBalance_AccountTypeExtendedSettingsConfig"; 
        public string Editor { get; set; }
    }
}
