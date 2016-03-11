using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.iTestIntegration
{
    public class ITestZone
    {
        public string ZoneId { get; set; }

        public string ZoneName { get; set; }

        public string CountryId { get; set; }

        public string CountryName { get; set; }

        public bool IsOffline { get; set; }

        public List<string> Codes { get; set; }
    }
}
