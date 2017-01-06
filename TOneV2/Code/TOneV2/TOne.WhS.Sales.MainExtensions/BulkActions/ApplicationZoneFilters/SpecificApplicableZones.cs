using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class SpecificApplicableZones : ApplicableZoneType
    {
        public override IEnumerable<long> GetApplicableZoneIds()
        {
            return base.SelectedZoneIds;
        }
    }
}
