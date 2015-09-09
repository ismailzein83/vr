using System;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class QueueNameAttribute : Attribute
    {

        public string QueueTitle { get; set; }
    }

    public enum QueueName
    {
        [QueueName(QueueTitle = "CDR Imported From Data Source {0}")]
        ImportedCDRs,
        [QueueName(QueueTitle = "CDR Parsed From Data Source {0}")]
        ParsedCDRs
    }
}
