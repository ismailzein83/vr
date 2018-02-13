using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV2.FieldParsers
{
    public class TimeFromTextParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("3CA92CC0-EAA7-4AA4-A5FC-77EBE5D940B7"); }
        }
        public string FieldName { get; set; }
        public string Format { get; set; }
        public override void Execute(IHexTLVFieldParserContext context)
        {
            TimeSpan timeSpan;
            string recordValue = context.Record.GetFieldValue(this.FieldName) as string;
            TimeSpan.TryParseExact(recordValue, this.Format, System.Globalization.CultureInfo.InvariantCulture, out timeSpan);
            if (timeSpan != null)
                context.Record.SetFieldValue(FieldName, new Time(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds));
        }
    }
}
