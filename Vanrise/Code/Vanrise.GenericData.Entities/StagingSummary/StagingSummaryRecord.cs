using System;

namespace Vanrise.GenericData.Entities
{
    public class StagingSummaryRecord
    {
        public long ProcessInstanceId { get; set; }
        public string StageName { get; set; }
        public DateTime BatchStart { get; set; }
        public DateTime BatchEnd { get; set; }
        public bool AlreadyFinalised { get; set; }
        public byte[] Data { get; set; }
    }
}