using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class WorkOrder
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string PathId { get; set; }
        public string NewPathId { get; set; }

        public string SupportsCommands { get; set; }
        public string Commands { get; set; }
        public string PhoneNumber { get; set; }
        public string ContractId { get; set; }
        public string SwitchName { get; set; }
        public string DeviceName { get; set; }
        public string OldDeviceId { get; set; }
        public string NewDeviceId { get; set; }
        public string Type { get; set; }
        public string NetworkServices { get; set; }
        public string SecondaryContracts { get; set; }
    }
}
