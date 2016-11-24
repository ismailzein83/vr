using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class SendSalePLNotifications : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<int>> CustomerIds { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SalePLZoneChange>> SalePLZonesChanges { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<int> customerIds = this.CustomerIds.Get(context);
            DateTime minimumDate = this.MinimumDate.Get(context);
            int sellingNumberPlanId = this.SellingNumberPlanId.Get(context);
            IEnumerable<SalePLZoneChange> salePLZonesChanges = this.SalePLZonesChanges.Get(context);

            INotificationContext notificationContext = new NotificationContext()
            {
                SellingNumberPlanId = sellingNumberPlanId,
                CustomerIds = customerIds,
                ZoneChanges = salePLZonesChanges,
                EffectiveDate = minimumDate,
                ChangeType = SalePLChangeType.CodeAndRate,
                InitiatorId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId
            };

            NotificationManager notificationManager = new NotificationManager();
            notificationManager.BuildNotifications(notificationContext);
        }

    }
}
