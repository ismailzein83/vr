using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;

namespace TOne.LCRProcess.Activities
{

    public sealed class GetTargetCustomerZoneRates : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ZoneCustomerRates> CustomerZoneRates { get; set; }

        [RequiredArgument]
        public InArgument<CodeCustomers> CodeCustomers { get; set; }

        [RequiredArgument]
        public OutArgument<ZoneCustomerRates> TargetZoneCustomerRates { get; set; }

        [RequiredArgument]
        public InArgument<CodeMatchesByCode> CodeMatchesByCode { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            ZoneCustomerRates newRates = new ZoneCustomerRates();

            CodeMatchesByCode codeMatches = this.CodeMatchesByCode.Get(context);
            CodeCustomers codeCustomers = this.CodeCustomers.Get(context);
            ZoneCustomerRates currentRates = this.CustomerZoneRates.Get(context);

            foreach (var customerRate in currentRates.ZonesCustomersRates)
            {

            }

        }
    }
}
