using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    public class SupplierZoneRateOutput
    {
        public SupplierZoneRates SupplierZoneRates { get; set; }
    }

    public class SupplierZoneRateInput
    {
        public HashSet<int> ZoneIds { get; set; }

        public int RoutingDatabaseId { get; set; }
    }

    public sealed class GetSupplierZoneRates : BaseAsyncActivity<SupplierZoneRateInput, SupplierZoneRateOutput>
    {

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<HashSet<int>> ZoneIds { get; set; }

        [RequiredArgument]
        public OutArgument<SupplierZoneRates> SupplierZoneRates { get; set; }

        protected override SupplierZoneRateOutput DoWorkWithResult(SupplierZoneRateInput inputArgument, AsyncActivityHandle handle)
        {
            SupplierZoneRates supplierRates = new SupplierZoneRates();
            IZoneRateDataManager dataManager = LCRDataManagerFactory.GetDataManager<IZoneRateDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            supplierRates = dataManager.GetSupplierZoneRates(inputArgument.ZoneIds);

            return new SupplierZoneRateOutput()
            {
                SupplierZoneRates = supplierRates
            };
        }

        protected override SupplierZoneRateInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new SupplierZoneRateInput()
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                ZoneIds = this.ZoneIds.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, SupplierZoneRateOutput result)
        {
            this.SupplierZoneRates.Set(context, result.SupplierZoneRates);
        }
    }
}
