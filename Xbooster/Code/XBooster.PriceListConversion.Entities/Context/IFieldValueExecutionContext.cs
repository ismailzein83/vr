using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBooster.PriceListConversion.Entities
{
    public interface IFieldValueExecutionContext
    {
         string Zone { get; }
         string Code  { get; }
         decimal Rate { get; }
         DateTime? EffectiveDate { get;  }
        Object FieldValue { set; }
    }
}
