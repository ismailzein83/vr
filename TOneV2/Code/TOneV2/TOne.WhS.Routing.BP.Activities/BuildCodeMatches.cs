using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{

    public class BuildCodeMatchesInput
    {
        public SupplierZoneDetailByZone SupplierZoneDetails { get; set; }

        public IEnumerable<SaleCode> SaleCodes { get; set; }

        public IEnumerable<SupplierCode> SupplierCodes { get; set; }
    }

    public class BuildCodeMatchesContext : IBuildCodeMatchesContext
    {
        public SupplierZoneDetailByZone SupplierZoneDetails { get; set; }
    }

    public sealed class BuildCodeMatches : BaseAsyncActivity<BuildCodeMatchesInput> 
    {
        [RequiredArgument]
        public InArgument<SupplierZoneDetailByZone> SupplierZoneDetails { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleCode>> SaleCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierCode>> SupplierCodes { get; set; }


        protected override void DoWork(BuildCodeMatchesInput inputArgument, AsyncActivityHandle handle)
        {
            CodeMatchBuilder builder = new CodeMatchBuilder();

            BuildCodeMatchesContext codeMatchContext = new BuildCodeMatchesContext();
            codeMatchContext.SupplierZoneDetails = inputArgument.SupplierZoneDetails;

            builder.BuildCodeMatches(codeMatchContext, inputArgument.SaleCodes, inputArgument.SupplierCodes, codeMatch => {
                Console.WriteLine(codeMatch.Code);
            });
        }

        protected override BuildCodeMatchesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchesInput
            {
                SaleCodes = this.SaleCodes.Get(context),
                SupplierCodes = this.SupplierCodes.Get(context),
                SupplierZoneDetails = this.SupplierZoneDetails.Get(context)
            };
        }
    }
}
