using System;
using Vanrise.Queueing.Entities;

namespace Vanrise.Integration.Entities
{
    public class DataSourceImportedBatch
    {
        public long ID { get; set; }
        public string BatchDescription { get; set; }
        public decimal BatchSize { get; set; }
        public BatchState BatchState { get; set; }
        public int RecordsCount { get; set; }
        public MappingResult MappingResult { get; set; }
        public string MapperMessage { get; set; }
        public string QueueItemIds { get; set; }
        public DateTime LogEntryTime { get; set; }
        public DateTime? BatchStart { get; set; }
        public DateTime? BatchEnd { get; set; }
        public ItemExecutionFlowStatus ExecutionStatus { get; set; }
    }
    public class DataSourceImportedBatchDetail
    {
        public long ID { get; set; }
        public string BatchDescription { get; set; }
        public decimal BatchSize { get; set; }
        public string BatchStateDescription { get; set; }
        public int RecordsCount { get; set; }
        public MappingResult MappingResult { get; set; }
        public string MappingResultDescription { get; set; }
        public string MapperMessage { get; set; }
        public string QueueItemIds { get; set; }
        public DateTime LogEntryTime { get; set; }
        public DateTime? BatchStart { get; set; }
        public DateTime? BatchEnd { get; set; }
        public ItemExecutionFlowStatus ExecutionStatus { get; set; }
    }
}