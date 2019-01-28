using Vanrise.Entities;

namespace Vanrise.Integration.Entities.Receive
{
    public class DataSourceImportedBatchProcessStateSettings : ProcessStateSettings
    {
        public long LastFinalizedId { get; set; }
    }
}