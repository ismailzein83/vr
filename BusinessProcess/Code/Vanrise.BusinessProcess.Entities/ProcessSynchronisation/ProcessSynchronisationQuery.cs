using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public enum ProcessSynchronisationStatus { Enabled = 0, Disabled = 1 }

    public class ProcessSynchronisationQuery
    {
        public string Name { get; set; }
        public List<ProcessSynchronisationStatus> Statuses { get; set; }
    }
}