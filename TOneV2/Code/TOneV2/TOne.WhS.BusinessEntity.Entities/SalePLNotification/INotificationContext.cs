using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface INotificationContext
    {
        int SellingNumberPlanId { get; }

        IEnumerable<int> CustomerIds { get; }

        IEnumerable<SalePLZoneChange> ZoneChanges { get; }

        DateTime EffectiveDate { get; }

        SalePLChangeType ChangeType { get; }

        int InitiatorId { get; }
    }
}
