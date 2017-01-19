using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class FirstOptionTQIMethod : TQIMethod
    {
        public override Guid ConfigId { get { return new Guid("6E7C4889-6977-4B8F-99AC-319094E2E5E7"); } }
        public decimal FirstOption { get; set; }
        public override void CalculateRate(ITQIMethodContext context)
        {
            context.Rate = this.FirstOption;
        }
    }
}
