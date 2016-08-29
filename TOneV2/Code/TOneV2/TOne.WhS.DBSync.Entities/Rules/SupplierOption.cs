using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.WhS.DBSync.Entities
{
    public class SupplierOption
    {
        public string SupplierId { get; set; }
        public short Priority { get; set; }
        public short Percentage { get; set; }
        public bool IsLoss { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", this.SupplierId, this.IsLoss, this.Percentage, this.Priority);
        }
        public override int GetHashCode()
        {
            return this.GetHashCode();
        }
    }
}
