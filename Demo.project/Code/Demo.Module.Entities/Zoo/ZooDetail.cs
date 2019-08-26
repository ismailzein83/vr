using System;

namespace Demo.Module.Entities
{
    public class ZooDetail
    {
        public long ZooId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public ZooSizeEnum Size { get; set; }
    }
}
