using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Entities
{
    public class OnNetOperatorDeclaration
    {
        public long ID { get; set; }
        public long OperatorID { get; set; }
        public int PeriodID { get; set; }
        public OnNetOperatorDeclarationServices OperatorDeclarationServices { get; set; }
    }
}
