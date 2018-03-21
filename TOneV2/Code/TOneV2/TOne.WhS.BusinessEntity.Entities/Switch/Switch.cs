using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BaseSwitch
    {
        public int SwitchId { get; set; }

        public string Name { get; set; }

        public SwitchSettings Settings { get; set; }

        public string SourceId { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }
    }

    public class Switch : BaseSwitch
    {
        
    }

    public class SwitchToEdit : BaseSwitch
    {

    }

    public class SwitchSettings
    {
        public SwitchRouteSynchronizer RouteSynchronizer { get; set; }

        public SwitchCDRMappingConfiguration SwitchCDRMappingConfiguration { get; set; }
    }
}
