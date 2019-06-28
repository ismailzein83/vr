using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.IVSwitch
{
	public class BuiltInIVSwitchSWSync : BaseIVSwitchSWSync
	{
		public string CdrConnectionString { get; set; }
		public override Guid ConfigId { get { return new Guid("1EE51230-FE31-4D01-9289-0E27E24D3601"); } }
		public override PreparedConfiguration GetPreparedConfiguration()
		{
			return PreparedConfiguration.GetBuiltInCachedPreparedConfiguration(this);
		}
		public override List<EndPointStatus> PrepareEndPointStatus(string carrierId, List<int> endPointStatuses)
		{
			IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
			int customerId;
			if (!int.TryParse(carrierId, out customerId)) return null;

			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			EndPointCarrierAccountExtension extendedSettingsObject = carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(customerId);

			if (extendedSettingsObject == null) return null;

			List<EndPointInfo> acl = extendedSettingsObject.AclEndPointInfo != null && extendedSettingsObject.AclEndPointInfo.Count > 0
				? extendedSettingsObject.AclEndPointInfo
				: null;

			List<EndPointInfo> sip = extendedSettingsObject.UserEndPointInfo != null && extendedSettingsObject.UserEndPointInfo.Count > 0
	? extendedSettingsObject.UserEndPointInfo
	: null;
			List<EndPointInfo> aclAndSipEndPoints;
			MergeAclSipEndPoints(acl, sip, out aclAndSipEndPoints);
			string endPointsString = null;
			if (aclAndSipEndPoints != null && aclAndSipEndPoints.Any())
				endPointsString = string.Join(",", aclAndSipEndPoints.Select(it => it.EndPointId));
			string query = string.Format("and user_id in ({0})", endPointsString);

			return masterDataManager.GetEndPointsStatus(query, endPointStatuses);
		}

		public override List<RouteStatus> PrepareRouteStatus(string carrierId, List<int> routestatuses)
		{
			IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
			int supplierId;
			if (!int.TryParse(carrierId, out supplierId)) return null;

			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			RouteCarrierAccountExtension extendedSettingsObject = carrierAccountManager.GetExtendedSettings<RouteCarrierAccountExtension>(supplierId);

			if (extendedSettingsObject == null) return null;

			List<RouteInfo> acl = extendedSettingsObject.RouteInfo != null && extendedSettingsObject.RouteInfo.Count > 0
				? extendedSettingsObject.RouteInfo
				: null;
			string routesString = null;
			if (acl != null && acl.Any())
				routesString = string.Join(",", acl.Select(it => it.RouteId));
			string query = string.Format("and route_id in ({0})", routesString);

			return masterDataManager.GetRouteStatus(query, routestatuses);
		}

		public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
		{
			CdrConnectionString = null;
			base.RemoveConnection(context);
		}
		private void MergeAclSipEndPoints(List<EndPointInfo> acl, List<EndPointInfo> sip, out List<EndPointInfo> mergedList)
		{
			mergedList = new List<EndPointInfo>();
			if (acl != null)
			{
				foreach (var endPoint in acl)
				{
					mergedList.Add(endPoint);
				}
			}
			if (sip != null)
			{
				foreach (var endPoint in sip)
				{
					mergedList.Add(endPoint);
				}
			}
		}
	}
}
