using System;

namespace Retail.RA.Entities
{
    public class IntlOperatorDeclaration
    {
        public long ID { get; set; }
        public long OperatorId { get; set; }
        public int PeriodId { get; set; }
        public IntlOperatorDeclarationServices OperatorDeclarationServices { get; set; }
    }
}
