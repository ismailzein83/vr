using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class SupplierZoneSelectionSet : BaseCodeSet
    {
        public List<int> SupplierZonesIds { get; set; }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }
    }
}
