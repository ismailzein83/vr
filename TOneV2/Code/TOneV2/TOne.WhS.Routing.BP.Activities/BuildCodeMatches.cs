using System.Collections.Generic;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Business;
using Vanrise.Queueing;
using Vanrise.Entities;
using System.Linq;

namespace TOne.WhS.Routing.BP.Activities
{

    public class BuildCodeMatchesInput
    {
        public IEnumerable<CodePrefix> CodePrefixGroup { get; set; }

        public SupplierZoneDetailByZone SupplierZoneDetails { get; set; }

        public IEnumerable<CodePrefixSaleCodes> SaleCodes { get; set; }

        public IEnumerable<CodePrefixSupplierCodes> SupplierCodes { get; set; }

        public BaseQueue<CodeMatchesBatch> OutputQueue_1 { get; set; }

        public BaseQueue<CodeMatchesBatch> OutputQueue_2 { get; set; }

        public BaseQueue<RoutingCodeMatches> OutputQueueForCustomerRoutes { get; set; }

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
        public InArgument<IEnumerable<CodePrefix>> CodePrefixGroup { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsCustomerRoutesProcess { get; set; }

        [RequiredArgument]
        public InArgument<SupplierZoneDetailByZone> SupplierZoneDetails { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CodePrefixSaleCodes>> SaleCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CodePrefixSupplierCodes>> SupplierCodes { get; set; }

        public InOutArgument<BaseQueue<CodeMatchesBatch>> OutputQueue_1 { get; set; }

        public InOutArgument<BaseQueue<CodeMatchesBatch>> OutputQueue_2 { get; set; }

        public InOutArgument<BaseQueue<RoutingCodeMatches>> OutputQueueForCustomerRoutes { get; set; }

        protected override void DoWork(BuildCodeMatchesInput inputArgument, AsyncActivityHandle handle)
        {            
            CodeMatchesBatch codeMatchesBatch = null;
            if (inputArgument.OutputQueue_1 != null || inputArgument.OutputQueue_2 != null)
                codeMatchesBatch = new CodeMatchesBatch();

            BuildCodeMatchesContext codeMatchContext = new BuildCodeMatchesContext();
            codeMatchContext.SupplierZoneDetails = inputArgument.SupplierZoneDetails;
            

            CodeMatchBuilder builder = new CodeMatchBuilder();
            //MemoryQueue<CodeMatches> outputQueueForCustomerRoutes = inputArgument.OutputQueueForCustomerRoutes as MemoryQueue<CodeMatches>;

            foreach (CodePrefix code in inputArgument.CodePrefixGroup)
            {
                IEnumerable<SaleCode> saleCodes = inputArgument.SaleCodes.FirstOrDefault(itm => itm.CodePrefix == code).SaleCodes.OrderBy(itm => itm.BED);
                IEnumerable<SupplierCode> supplierCodes = inputArgument.SupplierCodes.FirstOrDefault(itm => itm.CodePrefix == code).SupplierCodes.OrderBy(itm => itm.BED);
                codeMatchContext.CodePrefix = code.Code;
                builder.BuildCodeMatches(codeMatchContext, saleCodes, supplierCodes, codeMatch =>
                {
                    codeMatch.CodePrefix = code.Code;
                    if (inputArgument.IsCustomerRoutesProcess && inputArgument.OutputQueueForCustomerRoutes != null && codeMatch.SaleCodeMatches!=null && codeMatch.SaleCodeMatches.Count > 0)
                    {
                        RoutingCodeMatches routingCodeMatches = new RoutingCodeMatches()
                        {
                            Code = codeMatch.Code,
                            //CodePrefix = codeMatch.CodePrefix,
                            SupplierCodeMatches = codeMatch.SupplierCodeMatches,
                            SupplierCodeMatchesBySupplier = codeMatch.SupplierCodeMatchesBySupplier,
                            SaleZoneDefintions = new List<SaleZoneDefintion>()
                        };

                        foreach (SaleCodeMatch saleCodeMatch in codeMatch.SaleCodeMatches)
                        {
                            SaleZoneDefintion saleZoneDefintion = new SaleZoneDefintion() 
                            {
                                SaleZoneId = saleCodeMatch.SaleZoneId,
                                SellingNumberPlanId = saleCodeMatch.SellingNumberPlanId
                            };
                            routingCodeMatches.SaleZoneDefintions.Add(saleZoneDefintion);
                        }
                        inputArgument.OutputQueueForCustomerRoutes.Enqueue(routingCodeMatches);
                    }

                    if (codeMatchesBatch != null)
                    {
                        codeMatchesBatch.CodeMatches.Add(codeMatch);

                        if (codeMatchesBatch.CodeMatches.Count >= 10)
                        {
                            if (inputArgument.OutputQueue_1 != null)
                                inputArgument.OutputQueue_1.Enqueue(codeMatchesBatch);
                            if (inputArgument.OutputQueue_2 != null)
                                inputArgument.OutputQueue_2.Enqueue(codeMatchesBatch);
                            codeMatchesBatch = new CodeMatchesBatch();
                        }
                    }
                });


                if (codeMatchesBatch != null && codeMatchesBatch.CodeMatches.Count > 0)
                {
                    if (inputArgument.OutputQueue_1 != null)
                        inputArgument.OutputQueue_1.Enqueue(codeMatchesBatch);
                    if (inputArgument.OutputQueue_2 != null)
                        inputArgument.OutputQueue_2.Enqueue(codeMatchesBatch);
                    codeMatchesBatch = new CodeMatchesBatch();
                }
            }
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Building Code Matches is done", null);
        }

        protected override BuildCodeMatchesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchesInput
            {
                CodePrefixGroup = this.CodePrefixGroup.Get(context),
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
            //if (this.OutputQueue_1.Get(context) == null)
            //    this.OutputQueue_1.Set(context, new MemoryQueue<CodeMatchesBatch>());

            //if (this.OutputQueue_2.Get(context) == null)
            //    this.OutputQueue_2.Set(context, new MemoryQueue<CodeMatchesBatch>());

            //if (this.OutputQueueForCustomerRoutes.Get(context) == null)
            //    this.OutputQueueForCustomerRoutes.Set(context, new MemoryQueue<CodeMatches>());

            base.OnBeforeExecute(context, handle);
        }
    }
}