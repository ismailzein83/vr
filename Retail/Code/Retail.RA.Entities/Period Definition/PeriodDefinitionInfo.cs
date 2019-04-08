using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Entities
{
    public class PeriodDefinitionInfo
    {
        public int PeriodDefinitionId { get; set; }
        public string PeriodDefinitionName { get; set; }
        public bool HasDeclaration { get; set; }
    }
}
