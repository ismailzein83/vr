using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceSpecialRequest : SourceBaseRule
    {
        public string CustomerId { get; set; }
        public SpecialRequestSupplierOption SupplierOption { get; set; }
    }

    public class SpecialRequestSupplierOption
    {
        public string SupplierId { get; set; }
        public byte NumberOfTries { get; set; }
        public byte Percentage { get; set; }
        public byte Priority { get; set; }
        public bool ForcedOption { get; set; }
    }
}
