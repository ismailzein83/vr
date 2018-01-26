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

    public class ApplyChangedCustomerCountriesToDBInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public IEnumerable<ChangedCustomerCountry> ChangedCustomerCountries { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyChangedCustomerCountriesToDBOutput
    {

    }

    #endregion

    public class ApplyChangedCustomerCountriesToDB : Vanrise.BusinessProcess.BaseAsyncActivity<ApplyChangedCustomerCountriesToDBInput, ApplyChangedCustomerCountriesToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<ChangedCustomerCountry>> ChangedCustomerCountries { get; set; }

        #endregion

        protected override ApplyChangedCustomerCountriesToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyChangedCustomerCountriesToDBInput()
            {
                OwnerType = OwnerType.Get(context),
                ChangedCustomerCountries = ChangedCustomerCountries.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyChangedCustomerCountriesToDBOutput DoWorkWithResult(ApplyChangedCustomerCountriesToDBInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            IEnumerable<ChangedCustomerCountry> changedCustomerCountries = inputArgument.ChangedCustomerCountries;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
            
            if (ownerType == SalePriceListOwnerType.Customer)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<IChangedCustomerCountryDataManager>();
                dataManager.ProcessInstanceId =rootProcessInstanceId;
                dataManager.ApplyChangedCustomerCountriesToDB(changedCustomerCountries);
            }

            return new ApplyChangedCustomerCountriesToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyChangedCustomerCountriesToDBOutput result)
        {

        }
    }
}
