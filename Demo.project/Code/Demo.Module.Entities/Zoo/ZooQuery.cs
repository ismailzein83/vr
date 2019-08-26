using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class ZooQuery
    {
        public string Name { get; set; }
        public List<ZooSizeEnum> Sizes { get; set; }
    }
}
