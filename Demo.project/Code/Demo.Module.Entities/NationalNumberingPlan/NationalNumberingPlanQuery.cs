using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
   public class NationalNumberingPlanQuery
    {
       public List<int> NationalNumberingPlanIds { get; set; }

       public DateTime? FromDate { get; set; }

       public DateTime? ToDate { get; set; }

       public List<int> OperatorIds { get; set; }

    }
}
