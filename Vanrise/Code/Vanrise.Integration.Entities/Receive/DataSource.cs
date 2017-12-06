using System;
using System.Collections.Generic;

namespace Vanrise.Integration.Entities
{
    public class DataSource
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "VR_Integration_DataSource";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("E522907C-AEBD-48B6-82F4-FE55238942F2");

        public Guid DataSourceId { get; set; }

        public string Name { get; set; }

        public Guid AdapterTypeId { get; set; }

        public BaseAdapterState AdapterState { get; set; }

        public Guid TaskId { get; set; }

        public bool IsEnabled { get; set; }

        public DataSourceSettings Settings { get; set; }
    }

    public class DataSourceSettings
    {
        public BaseAdapterArgument AdapterArgument { get; set; }

        public DataSourceMappingSettings MappingSettings { get; set; }

        public string MapperCustomCode { get; set; }

        public Guid ExecutionFlowId { get; set; }

        public Guid? ErrorMailTemplateId { get; set; }
    }

    public class DataSourceMappingSettings
    {
        public Guid DataRecordTypeId { get; set; }

        public DataSourceParser Parser { get; set; }

        public List<string> ParsedRecordStages { get; set; }

        public DataSourceTransformationSettings TransformationSettings { get; set; }
    }

    public class DataSourceTransformationSettings
    {
        public int TransformationDefinitionId { get; set; }

        public string InputRecordName { get; set; }

        public List<DataSourceTransformationRecordStage> OutputRecordStages { get; set; }
    }

    public class DataSourceTransformationRecordStage
    {
        public string RecordName { get; set; }

        public List<string> StageNames { get; set; }
    }

    public abstract class DataSourceParser
    {
        public abstract void Execute(IDataSourceParserContext context);
    }

    public interface IDataSourceParserContext
    {
        IImportedData ImportedData { get; }

        List<dynamic> ParsedRecords { set; }
    }

    public class DataSourceManagmentInfo
    {
        public bool ShowEnableAll { get; set; }

        public bool ShowDisableAll { get; set; }

    }
}
