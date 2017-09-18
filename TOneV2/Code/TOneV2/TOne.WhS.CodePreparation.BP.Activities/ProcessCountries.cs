using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ProcessCountriesInput
    {
        public IEnumerable<ZoneToProcess> ZonesToProcess { get; set; }

        public int CountryId { get; set; }
    }

    public class ProcessCountriesOutput
    {
        public IEnumerable<ChangedCustomerCountry> ChangedCustomerCountries { get; set; }

    }
    public class ProcessCountries : BaseAsyncActivity<ProcessCountriesInput, ProcessCountriesOutput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ZoneToProcess>> ZonesToProcess { get; set; }

        [RequiredArgument]
        public InArgument<int> CountryId { get; set; }
        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedCustomerCountry>> ChangedCustomerCountries { get; set; }

        public int sellingNumberingPlanId;
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            CPParametersContext cpParametersContext = context.GetCPParameterContext() as CPParametersContext;
            sellingNumberingPlanId = cpParametersContext.SellingNumberPlanId;
            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessCountriesOutput DoWorkWithResult(ProcessCountriesInput inputArgument, AsyncActivityHandle handle)
        {
            
            IEnumerable<ZoneToProcess> zonesToProcess = inputArgument.ZonesToProcess;
            int countryId = inputArgument.CountryId;
            ProcessCountriesContext processCountriesContext = new ProcessCountriesContext()
            {
                ZonesToProcess = zonesToProcess,
                CountryId = countryId,
                SellingNumberPlanId = sellingNumberingPlanId
            };

            PriceListCountryManager plCountryManager = new PriceListCountryManager();
            plCountryManager.ProcessCountries(processCountriesContext);

            return new ProcessCountriesOutput()
            {
                ChangedCustomerCountries = processCountriesContext.ChangedCustomerCountries
            };
        }

        protected override ProcessCountriesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessCountriesInput()
            {
                ZonesToProcess = this.ZonesToProcess.Get(context),

                CountryId = this.CountryId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCountriesOutput result)
        {
            ChangedCustomerCountries.Set(context, result.ChangedCustomerCountries);
        }
    }
}
