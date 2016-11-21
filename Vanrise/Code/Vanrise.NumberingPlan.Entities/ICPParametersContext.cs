using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Entities
{
    public interface ICPParametersContext
    {
        DateTime EffectiveDate { get; }

        int SellingNumberPlanId { get; }

       ExistingZoneInfoByZoneName ExistingZonesInfoByZoneName { get; }
    }
}
