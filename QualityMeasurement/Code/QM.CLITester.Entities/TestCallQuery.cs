using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace QM.CLITester.Entities
{
    public class TestCallQuery
    {
        public List<CallTestStatus> CallTestStatus { get; set; }

        public List<CallTestResult> CallTestResult { get; set; }

        public List<int> UserIds { get; set; }

        public List<int> SupplierIds { get; set; }

        public List<int> ProfileIds { get; set; }
        public List<Guid> ScheduleIds { get; set; }

        public List<int> CountryIds { get; set; }

        public List<int> ZoneIds { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

    }
}
