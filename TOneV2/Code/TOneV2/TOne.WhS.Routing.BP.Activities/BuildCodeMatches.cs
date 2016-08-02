using System.Collections.Generic;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Business;
using Vanrise.Queueing;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class BuildCodeMatchesInput
    {
        public string CodePrefix { get; set; }

        public SupplierZoneDetailByZone SupplierZoneDetails { get; set; }

        public IEnumerable<SaleCode> SaleCodes { get; set; }

        public IEnumerable<SupplierCode> SupplierCodes { get; set; }

        public BaseQueue<CodeMatchesBatch> OutputQueue_1 { get; set; }

        public BaseQueue<CodeMatchesBatch> OutputQueue_2 { get; set; }

        public BaseQueue<CodeMatches> OutputQueueForCustomerRoutes { get; set; }
        
        public bool IsCustomerRoutesProcess { get; set; }
    }

    public class BuildCodeMatchesContext : IBuildCodeMatchesContext
    {
        public string CodePrefix { get; set; }

        public SupplierZoneDetailByZone SupplierZoneDetails { get; set; }
    }

    public sealed class BuildCodeMatches : BaseAsyncActivity<BuildCodeMatchesInput>
    {
        [RequiredArgument]
        public InArgument<string> CodePrefix { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsCustomerRoutesProcess { get; set; }

        [RequiredArgument]
        public InArgument<SupplierZoneDetailByZone> SupplierZoneDetails { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleCode>> SaleCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierCode>> SupplierCodes { get; set; }

        public InOutArgument<BaseQueue<CodeMatchesBatch>> OutputQueue_1 { get; set; }

        public InOutArgument<BaseQueue<CodeMatchesBatch>> OutputQueue_2 { get; set; }

        public InOutArgument<BaseQueue<CodeMatches>> OutputQueueForCustomerRoutes { get; set; }
        
        protected override void DoWork(BuildCodeMatchesInput inputArgument, AsyncActivityHandle handle)
        {
            CodeMatchesBatch codeMatchesBatch = new CodeMatchesBatch();

            BuildCodeMatchesContext codeMatchContext = new BuildCodeMatchesContext();
            codeMatchContext.SupplierZoneDetails = inputArgument.SupplierZoneDetails;
            codeMatchContext.CodePrefix = inputArgument.CodePrefix;

            CodeMatchBuilder builder = new CodeMatchBuilder();
            builder.BuildCodeMatches(codeMatchContext, inputArgument.SaleCodes, inputArgument.SupplierCodes, codeMatch =>
            {
                codeMatch.CodePrefix = inputArgument.CodePrefix;
                if (inputArgument.IsCustomerRoutesProcess)
                    inputArgument.OutputQueueForCustomerRoutes.Enqueue(codeMatch);

                codeMatchesBatch.CodeMatches.Add(codeMatch);

                if (codeMatchesBatch.CodeMatches.Count >= 10)
                {
                    inputArgument.OutputQueue_1.Enqueue(codeMatchesBatch);
                    inputArgument.OutputQueue_2.Enqueue(codeMatchesBatch);
                    codeMatchesBatch = new CodeMatchesBatch();
                }
            });

            if (codeMatchesBatch.CodeMatches.Count > 0)
            {
                inputArgument.OutputQueue_1.Enqueue(codeMatchesBatch);
                inputArgument.OutputQueue_2.Enqueue(codeMatchesBatch);
            }

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Building Code Matches is done", null);
        }

        protected override BuildCodeMatchesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchesInput
            {
                CodePrefix = this.CodePrefix.Get(context),
                IsCustomerRoutesProcess = this.IsCustomerRoutesProcess.Get(context),
                SaleCodes = this.SaleCodes.Get(context),
                SupplierCodes = this.SupplierCodes.Get(context),
                SupplierZoneDetails = this.SupplierZoneDetails.Get(context),
                OutputQueue_1 = this.OutputQueue_1.Get(context),
                OutputQueue_2 = this.OutputQueue_2.Get(context),
                OutputQueueForCustomerRoutes = this.OutputQueueForCustomerRoutes.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue_1.Get(context) == null)
                this.OutputQueue_1.Set(context, new MemoryQueue<CodeMatchesBatch>());

            if (this.OutputQueue_2.Get(context) == null)
                this.OutputQueue_2.Set(context, new MemoryQueue<CodeMatchesBatch>());

            if (this.OutputQueueForCustomerRoutes.Get(context) == null)
                this.OutputQueueForCustomerRoutes.Set(context, new MemoryQueue<CodeMatches>());

            base.OnBeforeExecute(context, handle);
        }
    }
}
