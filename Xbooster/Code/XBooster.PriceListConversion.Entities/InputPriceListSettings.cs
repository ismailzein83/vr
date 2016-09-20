using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace XBooster.PriceListConversion.Entities
{
    public abstract class InputPriceListSettings
    {
        public abstract Guid ConfigId { get;}

        public abstract PriceList Execute(IInputPriceListExecutionContext context);
    }
}
