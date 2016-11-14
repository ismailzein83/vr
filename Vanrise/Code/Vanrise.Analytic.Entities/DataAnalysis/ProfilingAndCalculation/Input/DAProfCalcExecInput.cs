using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcExecInput
    {
        public Guid OutputItemDefinitionId { get; set; }

        public Dictionary<Guid, dynamic> FilterParameterValues { get; set; }
    }
}
