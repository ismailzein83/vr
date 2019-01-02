using System;

namespace Vanrise.Integration.Entities
{
    public class ImportedBatchEntry
    {
        public long ID { get; set; }

        public string BatchDescription { get; set; }

        public decimal? BatchSize { get; set; }

        public BatchState BatchState { get; set; }

        public bool? IsDuplicateSameSize { get; set; }

        public int RecordsCount { get; set; }

        public MappingResult Result { get; set; }

        public string MapperMessage { get; set; }

        public string QueueItemsIds { get; set; }

        public DateTime? BatchStart { get; set; }

        public DateTime? BatchEnd { get; set; }
    }

    public enum BatchState { Normal = 0, Missing = 1, Delayed = 2, Duplicated = 3 }
}