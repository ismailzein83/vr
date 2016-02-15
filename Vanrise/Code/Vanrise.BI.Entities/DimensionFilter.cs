using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class DimensionFilter
    {
        public string EntityName { get; set; }
        public List<Object> Values { get; set; }
    }
}
