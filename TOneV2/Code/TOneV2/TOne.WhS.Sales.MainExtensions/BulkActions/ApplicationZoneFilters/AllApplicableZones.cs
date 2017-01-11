using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class AllApplicableZones : BulkActionZoneFilter
    {
        public override IEnumerable<long> GetApplicableZoneIds()
        {
            if (base.Action == null)
                throw new MissingMemberException("Action");

            //Get All sale Zone ids that are shown in rate plan grid without any filters
            //////////////////
            IEnumerable<long> saleZoneIds = null;
            
            List<long> applicableZoneIds = new List<long>();

            foreach (long id in saleZoneIds)
            {
                if (base.Action.IsZoneApplicable(id))
                    applicableZoneIds.Add(id);
            }

            return applicableZoneIds;
        }
    }
}
