using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class ProductQuery
    {
        public string Name { get; set; }

        public List<int> ManufactoryIds { get; set; }
    }
}