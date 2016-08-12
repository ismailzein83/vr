using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcOutputRecord
    {
        public Guid OutputItemDefinitionId { get; set; }

        public dynamic Record { get; set; }
    }
}
