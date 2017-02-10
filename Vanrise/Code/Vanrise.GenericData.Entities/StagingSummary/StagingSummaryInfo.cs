using System;

namespace Vanrise.GenericData.Entities
{
    public class StagingSummaryInfo
    {
        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public bool AlreadyFinalised { get; set; }
    }
}
