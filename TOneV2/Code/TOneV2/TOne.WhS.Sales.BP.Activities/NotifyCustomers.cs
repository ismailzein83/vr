using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
	public class NotifyCustomers : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<IEnumerable<int>> CustomerIds { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			IEnumerable<int> customerIds = CustomerIds.Get(context);
			int initiatorId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
			long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

			NotificationManager notificationManager = new NotificationManager();
			notificationManager.SendNotification(initiatorId, customerIds, processInstanceId);
		}
	}
}
