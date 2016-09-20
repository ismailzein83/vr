using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Entities
{
    public class SetAreaConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "PSTN_BE_SetArea";
        public string Editor { get; set; }
    }
}
