using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPortal.BusinessEntity.Entities
{
    public class CloudApplicationToAdd
    {
        public string Name { get; set; }

        public CloudApplicationSettings Settings { get; set; }
    }
}
