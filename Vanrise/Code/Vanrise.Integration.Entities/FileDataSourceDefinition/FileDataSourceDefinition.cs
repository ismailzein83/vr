using System;

namespace Vanrise.Integration.Entities
{
    public class FileDataSourceDefinition
    {
        public Guid FileDataSourceDefinitionId { get; set; }

        public string Name { get; set; }

        public TimeSpan DuplicateCheckInterval { get; set; } //2Days

        public FileDelayChecker FileDelayChecker { get; set; }

        public FileMissingChecker FileMissingChecker { get; set; }
    }
}