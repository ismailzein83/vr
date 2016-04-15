using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBooster.PriceListConversion.MainExtensions.OutputFieldValue
{
    public class Constant : Entities.OutputFieldValue
    {
        public string Value { get; set; }

        public override void Execute(Entities.IFieldValueExecutionContext context)
        {
            context.FieldValue = this.Value;
        }
    }
}
