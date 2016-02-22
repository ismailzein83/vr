using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
   public class OperatorConfigurationQuery
    {
       public List<int> OperatorConfigurationIds { get; set; }
       public List<int> OperatorIds { get; set; }
       public List<int> CDRDirectionIds { get; set; }

    }
}
