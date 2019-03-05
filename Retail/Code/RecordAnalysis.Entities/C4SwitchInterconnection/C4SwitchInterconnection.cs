using System;
using System.Collections.Generic;

namespace RecordAnalysis.Entities
{
    public abstract class BaseC4SwitchInterconnectionEntity
    {
        public int SwitchId { get; set; }
        public int InterconnectionId { get; set; }
        public C4SwitchInterconnection Settings { get; set; }
    }

    public class C4SwitchInterconnectionEntity : BaseC4SwitchInterconnectionEntity
    {
        public int SwitchInterconnectionId { get; set; }
    }

    public class C4SwitchInterconnectionEntityToSave : BaseC4SwitchInterconnectionEntity
    {
        public int? SwitchInterconnectionId { get; set; }
    }

    public class C4SwitchInterconnection
    {
        public List<C4SwitchInterconnectionTrunk> Trunks { get; set; }
        public List<C4SwitchInterconnectionTrunkGroups> TrunkGroups { get; set; }
    }

    public class C4SwitchInterconnectionTrunk
    {
        public Guid TrunkId { get; set; }
        public string TrunkName { get; set; }
    }

    public class C4SwitchInterconnectionTrunkGroups
    {
        public string Name { get; set; }
        public List<C4SwitchInterconnectionTrunk> Trunks { get; set; }
    }

    public class C4SwitchInterconnectionByTrunk : Dictionary<string, C4SwitchInterconnectionEntity> { }
}