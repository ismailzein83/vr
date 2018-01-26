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

    public class ApplyNewCustomerCountriesToDBInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public IEnumerable<NewCustomerCountry> NewCustomerCountries { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyNewCustomerCountriesToDBOutput
    {

    }

    #endregion

    public class ApplyNewCustomerCountriesToDB : Vanrise.BusinessProcess.BaseAsyncActivity<ApplyNewCustomerCountriesToDBInput, ApplyNewCustomerCountriesToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<NewCustomerCountry>> NewCustomerCountries { get; set; }

        #endregion

        protected override ApplyNewCustomerCountriesToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyNewCustomerCountriesToDBInput()
            {
                OwnerType = OwnerType.Get(context),
                NewCustomerCountries = NewCustomerCountries.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyNewCustomerCountriesToDBOutput DoWorkWithResult(ApplyNewCustomerCountriesToDBInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            IEnumerable<NewCustomerCountry> newCustomerCountries = inputArgument.NewCustomerCountries;

            if (ownerType == SalePriceListOwnerType.Customer)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<INewCustomerCountryDataManager>();
                long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
                dataManager.ProcessInstanceId = rootProcessInstanceId;
                dataManager.ApplyNewCustomerCountriesToDB(newCustomerCountries);
            }

            return new ApplyNewCustomerCountriesToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyNewCustomerCountriesToDBOutput result)
        {

        }
    }
}
