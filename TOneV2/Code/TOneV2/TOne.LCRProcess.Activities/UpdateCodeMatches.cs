using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;

namespace TOne.LCRProcess.Activities
{

    public sealed class UpdateCodeMatches : CodeActivity
    {
        public InArgument<bool> RebuildZoneRates { get; set; }
        public InArgument<SupplierZoneRates> SupplierZoneRates { get; set; }
        public InOutArgument<CodeMatchesByZoneId> CodeMatchesByZoneId { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            SupplierZoneRates supplierZoneRates = this.SupplierZoneRates.Get(context);
            CodeMatchesByZoneId codeMatchesByZoneId = this.CodeMatchesByZoneId.Get(context);
            CodeMatchesByZoneId codeMatchesFinal = new CodeMatchesByZoneId();
            foreach (var item in supplierZoneRates.RatesByZoneId)
            {
                               
            }
        }
    }
}
