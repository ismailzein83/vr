using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Entities
{
    public class IcxOperatorDeclaration
    {
        public long ID { get; set; }
        public long OperatorId { get; set; }
        public int PeriodId { get; set; }
        public IcxOperatorDeclarationServices OperatorDeclarationServices { get; set; }
    }
}
