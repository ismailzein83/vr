using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.AccountManager.Entities
{
  public class AccountManagerDefinitionConfig :ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_AccountManager_AccountManagerDefinition";
        public string Editor { get; set; }
    }
}
