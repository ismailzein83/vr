using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class  GetExistingZonesServicesInput
    {
        public int SellingNumberPlanId { get; set; }
        public DateTime MinimumDate { get; set; }
    }
    public class GetExistingZonesServicesOutput
    {
        public IEnumerable<SaleEntityZoneService> ExistingSaleEntityServicesEntities { get; set; }
    }
    public sealed class GetExistingZonesServices : Vanrise.BusinessProcess.BaseAsyncActivity<GetExistingZonesServicesInput, GetExistingZonesServicesOutput>
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanID { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleEntityZoneService>> ExistingSaleEntityZonesServices { get; set; }

        protected override GetExistingZonesServicesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingZonesServicesInput()
            {
                MinimumDate = this.MinimumDate.Get(context),
                SellingNumberPlanId = this.SellingNumberPlanID.Get(context),
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingSaleEntityZonesServices.Get(context) == null)
                this.ExistingSaleEntityZonesServices.Set(context, new List<SaleEntityZoneService>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetExistingZonesServicesOutput DoWorkWithResult(GetExistingZonesServicesInput inputArgument, AsyncActivityHandle handle)
        {

            SaleEntityServiceManager saleEntityServiceManager = new SaleEntityServiceManager();
            IEnumerable<SaleEntityZoneService> SaleEntityZonesServices = saleEntityServiceManager.GetSaleZonesServicesEffectiveAfter(inputArgument.SellingNumberPlanId, Vanrise.Common.Utilities.Min(inputArgument.MinimumDate, DateTime.Today));
            return new GetExistingZonesServicesOutput()
            {
                ExistingSaleEntityServicesEntities = SaleEntityZonesServices
            };
        }
        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingZonesServicesOutput result)
        {
            this.ExistingSaleEntityZonesServices.Set(context, result.ExistingSaleEntityServicesEntities);
        }
    }
}
