using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.RecurringCharges
{
    public class MonthlyRecuringCharge : RecurringChargePeriodSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A1D0F08B-B8F7-4B9C-9C83-49DABD2A68D5"); }
        }
        public int Day { get; set; }
        public override void Execute(IRecurringChargePeriodSettingsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
