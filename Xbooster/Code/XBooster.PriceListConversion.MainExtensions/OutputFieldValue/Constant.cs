using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBooster.PriceListConversion.MainExtensions.OutputFieldValue
{
    public class Constant : Entities.OutputFieldValue
    {
        public override Guid ConfigId { get { return new Guid("cc187d32-7c50-44b6-8bcb-790b06e5662c"); } }

        public string Value { get; set; }

        public override void Execute(Entities.IFieldValueExecutionContext context)
        {
            context.FieldValue = this.Value;
        }
    }
}
