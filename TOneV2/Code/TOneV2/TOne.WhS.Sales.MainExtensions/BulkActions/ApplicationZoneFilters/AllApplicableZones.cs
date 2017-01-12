using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class AllApplicableZones : BulkActionZoneFilter
    {
        public override IEnumerable<long> GetApplicableZoneIds(IApplicableZoneIdsContext context)
        {
            if (context.SaleZoneIds == null)
                throw new MissingMemberException("SaleZoneIds");

            List<long> applicableZoneIds = new List<long>();

            foreach (long zoneId in context.SaleZoneIds)
            {
                if (UtilitiesManager.IsActionApplicableToZone(context.BulkAction, zoneId, context.DraftData))
                    applicableZoneIds.Add(zoneId);
            }

            return applicableZoneIds;
        }
    }
}
