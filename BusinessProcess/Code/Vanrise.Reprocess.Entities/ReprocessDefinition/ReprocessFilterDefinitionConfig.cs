using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Reprocess.Entities
{
    public class ReprocessFilterDefinitionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Reprocess_ReprocessFilterDefinitionConfig";

        public string Editor { get; set; }
    }
}