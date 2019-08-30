using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class ZooSectionQuery
    {
        public string Name { get; set; }
        public List<long> ZooIds { get; set; }
        public int? MaxNbOfAnimals { get; set; }
    }
}
