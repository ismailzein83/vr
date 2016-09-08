using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Classes
    
    public class GetExistingSaleEntityZoneServicesInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingSaleEntityZoneServicesOutput
    {
        public IEnumerable<SaleEntityZoneService> ExistingSaleEntityZoneServices { get; set; }
    }
    
    #endregion

    public class GetExistingSaleEntityZoneServices : BaseAsyncActivity<GetExistingSaleEntityZoneServicesInput, GetExistingSaleEntityZoneServicesOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleEntityZoneService>> ExistingSaleEntityZoneServices { get; set; }

        #endregion

        protected override GetExistingSaleEntityZoneServicesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingSaleEntityZoneServicesInput()
            {
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingSaleEntityZoneServices.Get(context) == null)
                this.ExistingSaleEntityZoneServices.Set(context, new List<SaleEntityZoneService>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetExistingSaleEntityZoneServicesOutput DoWorkWithResult(GetExistingSaleEntityZoneServicesInput inputArgument, AsyncActivityHandle handle)
        {
            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            var saleEntityServiceManager = new SaleEntityServiceManager();
            IEnumerable<SaleEntityZoneService> saleEntityZoneServices = saleEntityServiceManager.GetZoneServicesEffectiveAfter(ownerType, ownerId, minimumDate);

            return new GetExistingSaleEntityZoneServicesOutput()
            {
                ExistingSaleEntityZoneServices = saleEntityZoneServices
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingSaleEntityZoneServicesOutput result)
        {
            this.ExistingSaleEntityZoneServices.Set(context, result.ExistingSaleEntityZoneServices);
        }
    }
}
