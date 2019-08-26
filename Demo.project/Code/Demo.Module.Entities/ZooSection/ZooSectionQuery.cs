using System;

namespace Demo.Module.Entities
{
    public class ZooSectionQuery
    {
        public string Name { get; set; }
        public long ZooId { get; set; }
        public ZooSectionType Type { get; set; }
        public int MaxNbOfAnimals { get; set; }
    }
}
