using System;
namespace Vanrise.Reprocess.Entities
{
    public class ReprocessDefinitionInfo
    {
        public Guid ReprocessDefinitionId { get; set; }

        public string Name { get; set; }

        public bool ForceUseTempStorage { get; set; }
    }
}
