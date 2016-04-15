using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBooster.PriceListConversion.MainExtensions.OutputFieldValue
{
    public class PriceListField : Entities.OutputFieldValue
    {
        public string FieldName { get; set; }
        public override void Execute(Entities.IFieldValueExecutionContext context)
        {
           switch(this.FieldName)
           {
               case "Code": context.FieldValue = context.Record.Code; break;
               case "EffectiveDate": context.FieldValue = context.Record.EffectiveDate; break;
               case "Rate": context.FieldValue = context.Record.Rate; break;
               case "Zone": context.FieldValue = context.Record.Zone; break;
           }
        }
    }
}
