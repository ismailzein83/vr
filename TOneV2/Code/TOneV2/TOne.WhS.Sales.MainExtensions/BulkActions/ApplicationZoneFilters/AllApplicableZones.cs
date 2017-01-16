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
		public override Guid ConfigId { get { return new Guid("BDC22FEB-14E1-4F0D-8C3E-EF54A5A36312"); } }

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
