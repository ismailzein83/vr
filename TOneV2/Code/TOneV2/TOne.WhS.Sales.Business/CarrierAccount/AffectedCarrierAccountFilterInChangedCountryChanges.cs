using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Business
{
    public class AffectedCarrierAccountFilterInChangedCountryChanges : ICarrierAccountFilter
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
                AffectedCustomerIds = new ChangedCustomerCountryPreviewManager().GetAffectedCustomerIds(processInstanceId);
            }
        }
    }
}
