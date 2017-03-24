using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.InvToAccBalanceRelation.Entities
{
    public class RelationDefinitionExtendedSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_InvToAccBalanceRelation_RelationDefinitionExtendedSettings";
        public string Editor { get; set; }
    }
}
