using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Sales.Business
{
    public class AffectedCarrierAccountFilterInRPChanges : ICarrierAccountFilter
    {
        public long ProcessInstanceId;

        public bool IsExcluded(ICarrierAccountFilterContext context)
        {
            if (context.CustomObject == null)
                context.CustomObject = new CustomObject(ProcessInstanceId);
            var customObject = context.CustomObject as CustomObject;
            return (customObject.AffectedCustomerIds == null || !customObject.AffectedCustomerIds.Contains(context.CarrierAccount.CarrierAccountId));
        }

        private class CustomObject
        {
            public IEnumerable<int> AffectedCustomerIds { get; set; }

            public CustomObject(long processInstanceId)
            {
                AffectedCustomerIds = new SalePriceListChangeManager().GetAffectedCustomerIdsRPChangesByProcessInstanceId(processInstanceId);
            }
        }
    }
}
