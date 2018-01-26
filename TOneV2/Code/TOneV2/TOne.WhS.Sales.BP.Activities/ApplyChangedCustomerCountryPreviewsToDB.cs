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

    public class ApplyChangedCustomerCountryPreviewsToDBInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public IEnumerable<ChangedCustomerCountryPreview> ChangedCustomerCountryPreviews { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyChangedCustomerCountryPreviewsToDBOutput
    {

    }

    #endregion

    public class ApplyChangedCustomerCountryPreviewsToDB : Vanrise.BusinessProcess.BaseAsyncActivity<ApplyChangedCustomerCountryPreviewsToDBInput, ApplyChangedCustomerCountryPreviewsToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedCustomerCountryPreview>> ChangedCustomerCountryPreviews { get; set; }

        #endregion

        protected override ApplyChangedCustomerCountryPreviewsToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyChangedCustomerCountryPreviewsToDBInput()
            {
                OwnerType = OwnerType.Get(context),
                ChangedCustomerCountryPreviews = ChangedCustomerCountryPreviews.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyChangedCustomerCountryPreviewsToDBOutput DoWorkWithResult(ApplyChangedCustomerCountryPreviewsToDBInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            IEnumerable<ChangedCustomerCountryPreview> changedCountryPreviews = inputArgument.ChangedCustomerCountryPreviews;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
            if (ownerType == SalePriceListOwnerType.Customer)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<IChangedCustomerCountryPreviewDataManager>();
                dataManager.ProcessInstanceId = rootProcessInstanceId;
                dataManager.ApplyChangedCustomerCountryPreviewsToDB(changedCountryPreviews);
            }

            return new ApplyChangedCustomerCountryPreviewsToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyChangedCustomerCountryPreviewsToDBOutput result)
        {

        }
    }
}
