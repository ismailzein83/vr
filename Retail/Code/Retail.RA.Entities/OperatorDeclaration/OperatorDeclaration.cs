using System;

namespace Retail.RA.Entities
{
    public class OperatorDeclaration
    {
        public long ID { get; set; }
        public long OperatorId { get; set; }
        public int PeriodId { get; set; }
        public OperatorDeclarationServices OperatorDeclarationServices { get; set; }
    }
}
