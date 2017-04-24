using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchSWSync : BaseIVSwitchSWSync
    {
        #region properties
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }
        public string Separator { get; set; }

        #endregion

        public override Guid ConfigId { get { return new Guid("64152327-5DB5-47AE-9569-23D38BCB18CC"); } }

        public override PreparedConfiguration GetPreparedConfiguration()
        {
            return PreparedConfiguration.GetCachedPreparedConfiguration(this);
        }
        public override List<EndPointStatus> PrepareEndPointStatus(string carrierId, List<int> endPointStatusIds)
        {
            CarrierMapping carrierMapping;
            List<string> accountIds = new List<string>();
            List<string> groupIds = new List<string>();
            if (!CarrierMappings.TryGetValue(carrierId, out carrierMapping)) return null;
            foreach (var mapping in carrierMapping.CustomerMapping)
            {
                string[] parts = mapping.Split('_');
                if (parts.Length > 2)
                {
                    accountIds.Add(parts[0]);
                    groupIds.Add(parts[1]);
                }
            }
            string accountIdsStr = null;
            if (accountIds.Any())
                accountIdsStr = string.Join(",", accountIds);
            string groupIdsStr = null;
            if (groupIds.Any())
                groupIdsStr = string.Join(",", groupIds);
            string query = string.Format("and account_id in( {0} ) AND  group_id in({1})", accountIdsStr, groupIdsStr);

            IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
            return masterDataManager.GetAccessListStatus(query, endPointStatusIds);
        }
    }
    public class CarrierMapping
    {
        public string CarrierId { get; set; }

        public List<string> CustomerMapping { get; set; }

        public List<string> SupplierMapping { get; set; }

    }
}
