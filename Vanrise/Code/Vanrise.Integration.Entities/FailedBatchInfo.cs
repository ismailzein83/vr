using System;

namespace Vanrise.Integration.Entities
{
    public class FailedBatchInfo
    {
        public string Message { get; set; }
        public bool IsEmpty { get; set; }
        public string BatchDescription { get; set; }
        public Guid DataSourceId { get; set; }
        public string DataSourceName { get; set; }

    }
}
