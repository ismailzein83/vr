using System;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities.EntitySynchronization;
namespace TOne.WhS.DBSync.Entities
{

    public class SourceCarrierSwitchConnectivity : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }
        public string CarrierAccountId { get; set; }
        public byte SwitchId { get; set; }
        public SwitchConnectivityType ConnectionType { get; set; }
        public int NumberOfChannelsIn { get; set; }
        public int NumberOfChannelsOut { get; set; }
        public int NumberOfChannelsShared { get; set; }
        public Single MarginTotal { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public string Details { get; set; }

    }
}
