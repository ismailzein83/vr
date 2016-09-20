using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBooster.PriceListConversion.MainExtensions.OutputFieldValue
{
    public class PriceListField : Entities.OutputFieldValue
    {
        public override Guid ConfigId { get { return new Guid("26cbdf89-7d0e-4ae0-bff4-949ef4cb1b65"); } }
        public string FieldName { get; set; }
        public override void Execute(Entities.IFieldValueExecutionContext context)
        {
           switch(this.FieldName)
           {
               case "Code": context.FieldValue = context.Code; break;
               case "EffectiveDate": context.FieldValue = context.EffectiveDate; break;
               case "Rate": context.FieldValue = context.Rate; break;
               case "Zone": context.FieldValue = context.Zone; break;
           }
        }
    }
}
