using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum HeaderFiledType
    {
        CustomerName = 0,
        PricelistType = 1,
        CompanyName = 2
    }

    public class HeaderMappedCell : MappedCell
    {
        public HeaderFiledType HeaderField { get; set; }

        public override void Execute(Entities.IMappedCellContext context)
        {
            throw new NotImplementedException();
        }
    }
}
