using System;
using System.Linq;
using System.Activities;
using Vanrise.BusinessProcess;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.BP.Activities
{
    public class GetNotificationtInput
    {
        public IEnumerable<int> CustomerIds { get; set; }
    }
    public class GetNotificationOutput
    {
        public Dictionary<int, SalePricelistNotification> SalePricelistNotificationsByCustomerId { get; set; }

    }
    public sealed class GetSalePricelistNotifications : BaseAsyncActivity<GetNotificationtInput, GetNotificationOutput>
    {
        [RequiredArgument]
        public InArgument<IEnumerable<int>> CustomerIds { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<int, SalePricelistNotification>> SalePricelistNotificationsByCustomerId { get; set; }

        protected override GetNotificationOutput DoWorkWithResult(GetNotificationtInput inputArgument, AsyncActivityHandle handle)
        {
            var salePricelistNotificationManager = new SalePricelistNotificationManager();
            IEnumerable<SalePricelistNotification> salePricelistNotifications = salePricelistNotificationManager.GetLastSalePricelistNotification(inputArgument.CustomerIds);

            return new GetNotificationOutput
            {
                SalePricelistNotificationsByCustomerId = StructureSalePricelistByCustomerId(salePricelistNotifications)
            };
        }

        protected override GetNotificationtInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetNotificationtInput
            {
                CustomerIds = this.CustomerIds.Get(context)
            };
        }
        private Dictionary<int, SalePricelistNotification> StructureSalePricelistByCustomerId(IEnumerable<SalePricelistNotification> salePricelistNotifications)
        {
            var salePricelistNotificationsByCustomerId = new Dictionary<int, SalePricelistNotification>();
            foreach (var pricelist in salePricelistNotifications)
            {
                if (salePricelistNotificationsByCustomerId.ContainsKey(pricelist.CustomerID))
                    salePricelistNotificationsByCustomerId.Add(pricelist.CustomerID, pricelist);
            }
            return salePricelistNotificationsByCustomerId;
        }
        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetNotificationOutput result)
        {
            SalePricelistNotificationsByCustomerId.Set(context, result.SalePricelistNotificationsByCustomerId);
        }
    }
}
