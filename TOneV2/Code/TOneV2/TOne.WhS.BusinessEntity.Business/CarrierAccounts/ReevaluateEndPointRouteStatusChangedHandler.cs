using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business.CarrierAccounts;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.Business.EventHandler
{
	public class ReevaluateEndPointRouteStatusChangedHandler : CarrierAccountStatusChangedEventHandler
	{
		public override Guid ConfigId
		{
			get { return new Guid("1366F843-DFCF-460D-922D-B3799BA69F58"); }
		}

		public override void Execute(IVREventHandlerContext context)
		{
			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			SwitchManager switchManager = new SwitchManager();

			var eventPayload = context.EventPayload as CarrierAccountStatusChangedEventPayload;
			var switches = switchManager.GetAllSwitches();
			eventPayload.ThrowIfNull("context.EventPayload", eventPayload);

			var carrierAccount = carrierAccountManager.GetCarrierAccount(eventPayload.CarrierAccountId);
			BlockAccountsOnSwitches(switches.ToList(), carrierAccount.CarrierAccountId, eventPayload);
		}
		private void BlockAccountsOnSwitches(List<Switch> switches, int carrierAccountId, CarrierAccountStatusChangedEventPayload eventPayload)
		{
			if (switches != null)
			{
				CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
				StringBuilder errorStringBuilder = new StringBuilder();
				foreach (var switchItem in switches)
				{
					try
					{
						var carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId);
						switchItem.Settings.ThrowIfNull("switchItem.Settings", switchItem.SwitchId);
						switchItem.Settings.RouteSynchronizer.ThrowIfNull("switchItem.Settings.RouteSynchronizer", switchItem.SwitchId);
						if (eventPayload.IsCarrierAccountStatusChanged)
						{
							if (carrierAccount.CarrierAccountSettings.ActivationStatus == ActivationStatus.Active)
							{
								TryReactivateContext context = new TryReactivateContext
								{
									CarrierAccountId = carrierAccountId.ToString()
								};
								switchItem.Settings.RouteSynchronizer.TryReactivate(context);
							}
							if (carrierAccount.CarrierAccountSettings.ActivationStatus == ActivationStatus.Inactive)
							{
								TryDeactivateContext context = new TryDeactivateContext
								{
									CarrierAccountId = carrierAccountId.ToString()
								};
								switchItem.Settings.RouteSynchronizer.TryDeactivate(context);
							}
						}

						if (eventPayload.IsCustomerRoutingStatusChanged)
						{
							if (carrierAccount.CustomerSettings.RoutingStatus == RoutingStatus.Blocked)
							{
								TryBlockCustomerContext context = new TryBlockCustomerContext
								{
									CustomerId = carrierAccountId.ToString(),
									SwitchName = switchItem.Name
								};
								switchItem.Settings.RouteSynchronizer.TryBlockCustomer(context);
							}

							if (carrierAccount.CustomerSettings.RoutingStatus == RoutingStatus.Enabled)
							{
								TryUnBlockCustomerContext context = new TryUnBlockCustomerContext
								{
									CustomerId = carrierAccountId.ToString(),
								};
								switchItem.Settings.RouteSynchronizer.TryUnBlockCustomer(context);
							}
						}

						if (eventPayload.IsSupplierRoutingStatusChanged)
						{

							if (carrierAccount.SupplierSettings.RoutingStatus == RoutingStatus.Blocked)
							{
								TryBlockSupplierContext context = new TryBlockSupplierContext
								{
									SupplierId = carrierAccountId.ToString(),
									SwitchName = switchItem.Name
								};
								switchItem.Settings.RouteSynchronizer.TryBlockSupplier(context);
							}
							if (carrierAccount.SupplierSettings.RoutingStatus == RoutingStatus.Enabled)
							{
								TryUnBlockSupplierContext context = new TryUnBlockSupplierContext
								{
									SupplierId = carrierAccountId.ToString(),
								};
								switchItem.Settings.RouteSynchronizer.TryUnBlockSupplier(context);
							}
						}
					}
					catch (Exception e)
					{
						string carrierAccountName = carrierAccountManager.GetCarrierAccountName(carrierAccountId);
						errorStringBuilder.AppendLine(string.Format("Couldn't Block Customer {0}.More Details: {1}",
							carrierAccountName, e.Message));
					}
				}
				if (errorStringBuilder.Length > 0)
					throw new VRBusinessException(errorStringBuilder.ToString());
			}
		}

	}
}
