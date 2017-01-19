using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class SuggestedPercentagesTQIMethod : TQIMethod
    {
        public override Guid ConfigId { get { return new Guid("C4D65558-F499-454E-906C-1B55E5DBC3CF"); } }
        public Dictionary<string,decimal> SuggestedPercentagesBySupplierName { get; set; }
        public override void CalculateRate(ITQIMethodContext context)
        {
            throw new NotImplementedException();
        }
    }
}
