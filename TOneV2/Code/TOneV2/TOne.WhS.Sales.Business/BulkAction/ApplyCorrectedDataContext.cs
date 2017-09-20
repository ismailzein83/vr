using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplyCorrectedDataContext : IApplyCorrectedDataContext
    {
        Func<long, ZoneChanges> _getZoneDraft;

        public ApplyCorrectedDataContext(Func<long, ZoneChanges> getZoneDraft)
        {
            _getZoneDraft = getZoneDraft;
        }

        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public BulkActionCorrectedData CorrectedData { get; set; }
        public int NewRateDayOffset { get; set; }
        public int IncreasedRateDayOffset { get; set; }
        public int DecreasedRateDayOffset { get; set; }

        public ZoneChanges GetZoneDraft(long zoneId)
        {
            return _getZoneDraft(zoneId);
        }
    }
}
