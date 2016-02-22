using System;
using System.Collections.Generic;

namespace Demo.Module.Entities
{
   public class OperatorDeclaredInformationQuery
    {
       public List<int> OperatorDeclaredInformationIds { get; set; }

       public DateTime? FromDate { get; set; }

       public DateTime? ToDate { get; set; }

       public List<int> OperatorIds { get; set; }

       public List<long> ZoneIds { get; set; }

       public List<int> ServiceTypeIds { get; set; }

    }
}
