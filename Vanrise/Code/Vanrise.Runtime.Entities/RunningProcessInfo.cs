﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class RunningProcessInfo
    {
        public int ProcessId { get; set; }

        public string ProcessName { get; set; }

        public string MachineName { get; set; }

        public DateTime StartedTime { get; set; }

        public RunningProcessAdditionalInfo AdditionalInfo { get; set; }
    }

    public class RunningProcessAdditionalInfo
    {
        public string TCPServiceURL { get; set; }

        public string ServicePortNumber { get; set; }
    }
}
