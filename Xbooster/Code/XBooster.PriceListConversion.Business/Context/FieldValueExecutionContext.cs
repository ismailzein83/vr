using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBooster.PriceListConversion.Entities;

namespace XBooster.PriceListConversion.Business
{
    public class FieldValueExecutionContext : IFieldValueExecutionContext
    {
        public string Zone { get; set; }
        public string Code { get; set; }
        public decimal Rate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public Object FieldValue { get; set; }
    }
}
