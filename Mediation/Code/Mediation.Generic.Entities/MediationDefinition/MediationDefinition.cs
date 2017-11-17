using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace Mediation.Generic.Entities
{
    public class MediationDefinition
    {
        public Guid MediationDefinitionId { get; set; }
        public string Name { get; set; }
        public Guid ParsedRecordTypeId { get; set; }
        public Guid CookedRecordTypeId { get; set; }
        public List<MediationOutputHandlerDefinition> OutputHandlers { get; set; }
        public Guid ExecutionFlowDefinitionId { get; set; }
        public ParsedRecordIdentificationSetting ParsedRecordIdentificationSetting { get; set; }
        public ParsedTransformationSettings ParsedTransformationSettings { get; set; }
        public BadCDRIdentificationSettings BadCDRIdentificationSettings { get; set; }
    }

    public class ParsedRecordIdentificationSetting
    {
        public string SessionIdField { get; set; }
        public string EventTimeField { get; set; }
        public TimeSpan? TimeOutInterval { get; set; }
        public List<StatusMapping> StatusMappings { get; set; }

    }

    public class ParsedTransformationSettings
    {
        public Guid TransformationDefinitionId { get; set; }

        public string ParsedRecordName { get; set; }
    }

    public class MediationOutputHandlerDefinition
    {
        public string OutputRecordName { get; set; }

        public MediationOutputHandler Handler { get; set; }
    }

    public class BadCDRIdentificationSettings
    {
        public RecordFilterGroup BadCDRFilterGroup { get; set; }
    }
}
