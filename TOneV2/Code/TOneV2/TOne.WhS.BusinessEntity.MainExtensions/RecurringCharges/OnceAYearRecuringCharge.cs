using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.RecurringCharges
{
    public class OnceAYearRecuringCharge : RecurringChargePeriodSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("447B67A5-DBEA-4410-9C00-C8D297B8F81C"); }
        }

        public override void Evaluate(IRecurringChargePeriodSettingsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
