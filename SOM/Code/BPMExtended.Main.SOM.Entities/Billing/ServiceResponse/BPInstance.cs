using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public enum BPInstanceStatus
    {
        New = 0,
        Postponed = 5,
        Running = 10,
        Waiting = 20,
        Cancelling = 30,
        Completed = 50,
        Aborted = 60,
        Suspended = 70,
        Terminated = 80,
        Cancelled = 90
    }

    public class BPInstance
    {
        public string Title { get; set; }
        public long ProcessInstanceID { get; set; }
        public long? ParentProcessID { get; set; }
        public int InitiatorUserId { get; set; }
        public string InputArgument { get; set; }
        public BPInstanceStatus Status { get; set; }
        public DateTime? StatusUpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
