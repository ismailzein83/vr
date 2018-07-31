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
            string endPointsString = null;
            if (acl != null && acl.Any())
                endPointsString = string.Join(",", acl.Select(it => it.EndPointId));
            string query = string.Format("and user_id in ({0})", endPointsString);

            return masterDataManager.GetAccessListStatus(query, endPointStatuses);
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            CdrConnectionString = null;
            base.RemoveConnection(context);
        }
    }
}
