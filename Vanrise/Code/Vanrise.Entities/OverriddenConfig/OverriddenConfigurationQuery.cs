using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
   public class OverriddenConfigurationQuery
    {
        public string Name { get; set; }

        public List<Guid> OverriddenConfigGroupIds { get; set; }
    }
}
