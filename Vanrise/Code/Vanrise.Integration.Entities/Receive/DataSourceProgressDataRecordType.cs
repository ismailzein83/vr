using System;

namespace Vanrise.Integration.Entities
{
    public class DataSourceSummary
    {
        public Guid DataSourceId { get; set; }
        public DateTime LastImportedBatchTime { get; set; }
        public int NbImportedBatch { get; set; }
        public int TotalRecordCount { get; set; }
        public int MaxRecordCount { get; set; }
        public int MinRecordCount { get; set; }
        public decimal? MaxBatchSize { get; set; }
        public decimal? MinBatchSize { get; set; }
        public int NbInvalidBatch { get; set; }
        public int NbEmptyBatch { get; set; }
    }
}
