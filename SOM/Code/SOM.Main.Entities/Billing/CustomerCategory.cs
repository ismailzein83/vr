using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{  
    public class CustomerCategory
    {
        public string CategoryId { get; set; }

        public string Name { get; set; }

        public CustomerType CustomerType { get; set; }

        public bool IsNormal { get; set; }
    }
}
