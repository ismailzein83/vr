using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class ZooSectionDetail
    {
        public long ZooSectionId { get; set; }
        public string Name { get; set; }
        public long ZooId { get; set; }
        public List<ZooSectionPositionEnum> Position { get; set; }

    }
}
