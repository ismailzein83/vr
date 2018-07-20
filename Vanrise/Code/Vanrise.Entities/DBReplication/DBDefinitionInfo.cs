using System;
using System.Collections.Generic;

namespace Vanrise.Entities
{
    public class DBDefinitionInfo
    {
        public Guid DBDefinitionId { get; set; }
        public string Name { get; set; }
    }

    public class DBDefinitionInfoFilter
    {
        public List<Guid> ExcludedDBDefinitionIds { get; set; }
        public List<Guid> ForceDBDefinitionIds { get; set; }
    }
}
