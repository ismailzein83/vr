using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Classes

    public class ApplyNewCustomerCountryPreviewsToDBInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public IEnumerable<NewCustomerCountryPreview> NewCustomerCountryPreviews { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyNewCustomerCountryPreviewsToDBOutput
    {

    }

    #endregion

    public class ApplyNewCustomerCountryPreviewsToDB : Vanrise.BusinessProcess.BaseAsyncActivity<ApplyNewCustomerCountryPreviewsToDBInput, ApplyNewCustomerCountryPreviewsToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewCustomerCountryPreview>> NewCustomerCountryPreviews { get; set; }

        #endregion

        protected override ApplyNewCustomerCountryPreviewsToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyNewCustomerCountryPreviewsToDBInput()
            {
                OwnerType = OwnerType.Get(context),
                NewCustomerCountryPreviews = NewCustomerCountryPreviews.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyNewCustomerCountryPreviewsToDBOutput DoWorkWithResult(ApplyNewCustomerCountryPreviewsToDBInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            IEnumerable<NewCustomerCountryPreview> newCustomerCountryPreviews = inputArgument.NewCustomerCountryPreviews;

            if (ownerType == SalePriceListOwnerType.Customer)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<INewCustomerCountryPreviewDataManager>();
                long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
                dataManager.ProcessInstanceId = rootProcessInstanceId;
                dataManager.ApplyNewCustomerCountryPreviewsToDB(newCustomerCountryPreviews);
            }

            return new ApplyNewCustomerCountryPreviewsToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyNewCustomerCountryPreviewsToDBOutput result)
        {

        }
    }
}
