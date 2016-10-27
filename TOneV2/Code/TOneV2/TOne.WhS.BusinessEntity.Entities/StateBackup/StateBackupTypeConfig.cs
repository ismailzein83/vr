using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StateBackupTypeConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_BE_StateBackupType";
        
        public string FilterEditor { get; set; }

        public StateBackupBehavior Behavior { get; set; }
    }
}
