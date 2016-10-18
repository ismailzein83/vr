using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSource
    {
        public int DataSourceId { get; set; }

        public string Name { get; set; }

        public int AdapterTypeId { get; set; }

        public string AdapterName { get; set; }

        public AdapterTypeInfo AdapterInfo { get; set; }

        public BaseAdapterState AdapterState { get; set; }

        public int TaskId { get; set; }

        public bool IsEnabled { get; set; }

        public DataSourceSettings Settings { get; set; }
    }

    public class DataSourceSettings
    {
        public BaseAdapterArgument AdapterArgument { get; set; }

        public DataSourceMappingSettings MappingSettings { get; set; }

        public string MapperCustomCode { get; set; }

        public Guid ExecutionFlowId { get; set; }
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
}
