using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class ValidateBeforeSaveContext : IValidateBeforeSaveContext
    {

        public List<string> ValidateMessages { get; set; }

        public bool IsEditMode { get; set; }
        public DealDefinition ExistingDeal { get; set; }
        public List<long> DealSaleZoneIds { get; set; }
        public List<long> DealSupplierZoneIds { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public int CustomerId { get; set; }
        public int? DealId { get; set; }
    }
}
