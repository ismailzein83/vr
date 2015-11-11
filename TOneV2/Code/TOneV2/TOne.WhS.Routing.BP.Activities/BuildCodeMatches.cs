using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Business;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{

    public class BuildCodeMatchesInput
    {
        public SupplierZoneDetailByZone SupplierZoneDetails { get; set; }

        public IEnumerable<SaleCode> SaleCodes { get; set; }

        public IEnumerable<SupplierCode> SupplierCodes { get; set; }

        public BaseQueue<CodeMatchesBatch> OutputQueue { get; set; }
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

        [RequiredArgument]
        public InOutArgument<BaseQueue<CodeMatchesBatch>> OutputQueue { get; set; }


        protected override void DoWork(BuildCodeMatchesInput inputArgument, AsyncActivityHandle handle)
        {
            CodeMatchesBatch codeMatchesBatch = new CodeMatchesBatch();

            BuildCodeMatchesContext codeMatchContext = new BuildCodeMatchesContext();
            codeMatchContext.SupplierZoneDetails = inputArgument.SupplierZoneDetails;

            CodeMatchBuilder builder = new CodeMatchBuilder();
            builder.BuildCodeMatches(codeMatchContext, inputArgument.SaleCodes, inputArgument.SupplierCodes, codeMatch => {
                codeMatchesBatch.CodeMatches.Add(codeMatch);
                if(codeMatchesBatch.CodeMatches.Count >= 10)
                {
                    inputArgument.OutputQueue.Enqueue(codeMatchesBatch);
                    codeMatchesBatch = new CodeMatchesBatch();
                }
            });

            if(codeMatchesBatch.CodeMatches.Count > 0)
                inputArgument.OutputQueue.Enqueue(codeMatchesBatch);
        }

        protected override BuildCodeMatchesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchesInput
            {
                SaleCodes = this.SaleCodes.Get(context),
                SupplierCodes = this.SupplierCodes.Get(context),
                SupplierZoneDetails = this.SupplierZoneDetails.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CodeMatchesBatch>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
