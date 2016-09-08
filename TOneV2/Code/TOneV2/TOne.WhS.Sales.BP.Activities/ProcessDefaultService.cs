using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Classes

    public class ProcessDefaultServiceInput
    {
        public DefaultServiceToAdd DefaultServiceToAdd { get; set; }

        public DefaultServiceToClose DefaultServiceToClose { get; set; }

        public IEnumerable<ExistingDefaultService> ExistingDefaultServices { get; set; }
    }

    public class ProcessDefaultServiceOutput
    {
        public NewDefaultService NewDefaultService { get; set; }

        public IEnumerable<ChangedDefaultService> ChangedDefaultServices { get; set; }
    }

    #endregion

    public class ProcessDefaultService : BaseAsyncActivity<ProcessDefaultServiceInput, ProcessDefaultServiceOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<DefaultServiceToAdd> DefaultServiceToAdd { get; set; }

        [RequiredArgument]
        public InArgument<DefaultServiceToClose> DefaultServiceToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingDefaultService>> ExistingDefaultServices { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<NewDefaultService> NewDefaultService { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedDefaultService>> ChangedDefaultServices { get; set; }

        #endregion

        protected override ProcessDefaultServiceInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessDefaultServiceInput()
            {
                DefaultServiceToAdd = this.DefaultServiceToAdd.Get(context),
                DefaultServiceToClose = this.DefaultServiceToClose.Get(context),
                ExistingDefaultServices = this.ExistingDefaultServices.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.NewDefaultService.Get(context) == null)
                this.NewDefaultService.Set(context, new NewDefaultService());

            if (this.ChangedDefaultServices.Get(context) == null)
                this.ChangedDefaultServices.Set(context, new List<ChangedDefaultService>());

            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessDefaultServiceOutput DoWorkWithResult(ProcessDefaultServiceInput inputArgument, AsyncActivityHandle handle)
        {
            DefaultServiceToAdd defaultServiceToAdd = inputArgument.DefaultServiceToAdd;
            DefaultServiceToClose defaultServiceToClose = inputArgument.DefaultServiceToClose;
            IEnumerable<ExistingDefaultService> existingDefaultServices = inputArgument.ExistingDefaultServices;

            ProcessDefaultServiceContext processDefaultServiceContext = new ProcessDefaultServiceContext()
            {
                DefaultServiceToAdd = defaultServiceToAdd,
                DefaultServiceToClose = defaultServiceToClose,
                ExistingDefaultServices = existingDefaultServices
            };

            var priceListDefaultServiceManager = new PriceListDefaultServiceManager();
            priceListDefaultServiceManager.ProcessDefaultService(processDefaultServiceContext);

            return new ProcessDefaultServiceOutput()
            {
                NewDefaultService = processDefaultServiceContext.NewDefaultService,
                ChangedDefaultServices = processDefaultServiceContext.ChangedDefaultServices
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessDefaultServiceOutput result)
        {
            this.NewDefaultService.Set(context, result.NewDefaultService);
            this.ChangedDefaultServices.Set(context, result.ChangedDefaultServices);
        }
    }
}
