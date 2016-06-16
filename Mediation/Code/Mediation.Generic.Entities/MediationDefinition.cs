using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Entities
{
    public class MediationDefinition
    {
        public int MediationDefinitionId { get; set; }
        public string Name { get; set; }
        public int ParsedRecordTypeId { get; set; }
        public int CookedRecordTypeId { get; set; }
        public UpdateCookedFromParsed CookedFromParsedSettings { get; set; }

    }

    public class UpdateCookedFromParsed
    {
        public int TransformationDefinitionId { get; set; }

        public string ParsedRecordName { get; set; }

        public string CookedRecordName { get; set; }
    }
}
