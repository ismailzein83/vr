using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business.CarrierAccounts;
using Vanrise.Entities;

namespace NP.IVSwitch.Business
{
	public class EndPointRouteStatusChangedHandler : CarrierAccountStatusChangedEventHandler
	{
		public override Guid ConfigId {
			get { return new Guid("C7001AB0-AF60-4BEC-BB9E-C91EAC6DDD28"); }
		}

		public override void Execute(IVREventHandlerContext context)
		{
			//throw new NotImplementedException();
		}
	}
}
