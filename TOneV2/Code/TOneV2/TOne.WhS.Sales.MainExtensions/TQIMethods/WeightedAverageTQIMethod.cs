using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class WeightedAverageTQIMethod : TQIMethod
    {
        public override Guid ConfigId { get { return new Guid("C75EA417-7D46-48C2-93DE-1B575373AAD2"); } }
        public decimal PeriodValue { get; set; }
        public PeriodTypes PeriodType { get; set; }
        public override void CalculateRate(ITQIMethodContext context)
        {
            throw new NotImplementedException();
        }
    }
}
