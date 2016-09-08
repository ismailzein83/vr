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

    public class GetExistingSaleEntityDefaultServicesInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingSaleEntityDefaultServicesOutput
    {
        public IEnumerable<SaleEntityDefaultService> ExistingSaleEntityDefaultServices { get; set; }
    }
    
    #endregion

    public class GetExistingSaleEntityDefaultServices : BaseAsyncActivity<GetExistingSaleEntityDefaultServicesInput, GetExistingSaleEntityDefaultServicesOutput>
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
        public OutArgument<IEnumerable<SaleEntityDefaultService>> ExistingSaleEntityDefaultServices { get; set; }

        #endregion

        protected override GetExistingSaleEntityDefaultServicesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingSaleEntityDefaultServicesInput()
            {
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingSaleEntityDefaultServices.Get(context) == null)
                this.ExistingSaleEntityDefaultServices.Set(context, new List<SaleEntityDefaultService>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetExistingSaleEntityDefaultServicesOutput DoWorkWithResult(GetExistingSaleEntityDefaultServicesInput inputArgument, AsyncActivityHandle handle)
        {
            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            var saleEntityServiceManager = new SaleEntityServiceManager();
            IEnumerable<SaleEntityDefaultService> defaultServices = saleEntityServiceManager.GetDefaultServicesEffectiveAfter(ownerType, ownerId, minimumDate);

            return new GetExistingSaleEntityDefaultServicesOutput
            {
                ExistingSaleEntityDefaultServices = defaultServices
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingSaleEntityDefaultServicesOutput result)
        {
            this.ExistingSaleEntityDefaultServices.Set(context, result.ExistingSaleEntityDefaultServices);
        }
    }
}
