using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class PriceListDefaultServiceManager
    {
        public void ProcessDefaultService(IProcessDefaultServiceContext context)
        {
            if (context.DefaultServiceToAdd != null)
                ProcessDefaultServiceToAdd(context.DefaultServiceToAdd, context.ExistingDefaultServices);
            else if (context.DefaultServiceToClose != null)
                ProcessDefaultServiceToClose(context.DefaultServiceToClose, context.ExistingDefaultServices);

            if (context.DefaultServiceToAdd != null)
                context.NewDefaultService = context.DefaultServiceToAdd.NewDefaultService;
            context.ChangedDefaultServices = context.ExistingDefaultServices.Where(x => x.ChangedDefaultService != null).Select(x => x.ChangedDefaultService);
        }

        private void ProcessDefaultServiceToAdd(DefaultServiceToAdd defaultServiceToAdd, IEnumerable<ExistingDefaultService> existingDefaultServices)
        {
            if (existingDefaultServices == null)
                return;

            foreach (ExistingDefaultService existingDefaultService in existingDefaultServices)
            {
                if (existingDefaultService.IsOverlappedWith(defaultServiceToAdd))
                {
                    existingDefaultService.ChangedDefaultService = new ChangedDefaultService()
                    {
                        SaleEntityServiceId = existingDefaultService.SaleEntityDefaultServiceEntity.SaleEntityServiceId,
                        EED = Utilities.Max(existingDefaultService.BED, defaultServiceToAdd.BED)
                    };
                    defaultServiceToAdd.ChangedExistingDefaultServices.Add(existingDefaultService);
                }
            }
        }

        private void ProcessDefaultServiceToClose(DefaultServiceToClose defaultServiceToClose, IEnumerable<ExistingDefaultService> existingDefaultServices)
        {
            if (existingDefaultServices == null)
                return;

            foreach (ExistingDefaultService existingDefaultService in existingDefaultServices)
            {
                if (existingDefaultService.EED.VRGreaterThan(defaultServiceToClose.CloseEffectiveDate))
                {
                    existingDefaultService.ChangedDefaultService = new ChangedDefaultService()
                    {
                        SaleEntityServiceId = existingDefaultService.SaleEntityDefaultServiceEntity.SaleEntityServiceId,
                        EED = Utilities.Max(existingDefaultService.BED, defaultServiceToClose.CloseEffectiveDate)
                    };
                    defaultServiceToClose.ChangedExistingDefaultServices.Add(existingDefaultService);
                }
            }
        }
    }
}
