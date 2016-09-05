using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PointOfSale
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public POSSetting Settings { get; set; }
        public string SourceId { get; set; }
    }
}
