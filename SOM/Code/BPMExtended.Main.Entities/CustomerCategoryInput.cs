using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CustomerCategoryInput
    {
        public string CustomerId { get; set; }
        public string CustomerCategoryId { get; set; }
        public CommonInputArgument CommonInputArgument { get; set; }
    }
}
