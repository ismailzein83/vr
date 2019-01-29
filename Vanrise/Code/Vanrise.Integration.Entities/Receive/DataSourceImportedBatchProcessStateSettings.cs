using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public class DataSourceImportedBatchProcessStateSettings : ProcessStateSettings
    {
        public long LastFinalizedId { get; set; }
    }
}