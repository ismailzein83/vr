using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
   public class OperatorConfigurationQuery
    {
       public List<int> OperatorConfigurationIds { get; set; }
       public List<int> OperatorIds { get; set; }
       public List<int> CDRDirectionIds { get; set; }
       public DateTime? FromDate { get; set; }
       public DateTime? ToDate { get; set; }

       public List<int> DestinationGroups { get; set; }

    }
}
