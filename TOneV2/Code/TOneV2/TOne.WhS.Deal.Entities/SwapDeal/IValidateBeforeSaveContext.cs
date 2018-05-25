using System;
using System.Collections.Generic;
using System.Linq;

namespace TOne.WhS.Deal.Entities
{
    public interface IValidateBeforeSaveContext
    {
        List<string> ValidateMessages { get; set; }
        bool IsEditMode { get;}
        DealDefinition ExistingDeal { get;}
         List<long> DealSaleZoneIds { get;}
         List<long> DealSupplierZoneIds { get;}
         DateTime BED { get;}
         DateTime? EED { get;}
         int CustomerId { get;}
         int? DealId { get;}
    }
}
