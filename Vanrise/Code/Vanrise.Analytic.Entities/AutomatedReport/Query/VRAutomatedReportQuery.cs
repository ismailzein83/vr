using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportQuery
    {
        public Guid DefinitionId { get; set; }

        public string QueryName { get; set; }

        public VRAutomatedReportQuerySettings Settings { get; set; }
    }
}
