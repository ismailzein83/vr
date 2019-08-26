using System;

namespace Demo.Module.Entities
{
    public enum ZooSizeEnum { Small, Medium, Large }

    public class Zoo
    {
        public long ZooId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public ZooSizeEnum Size { get; set; }
    }
}
