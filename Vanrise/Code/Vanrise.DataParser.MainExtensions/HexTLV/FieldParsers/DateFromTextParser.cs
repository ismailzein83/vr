using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class DateFromTextParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("2ADCD3A8-18E4-4CD0-B4AC-F644E4A72408"); }
        }

        public string DateFormat { get; set; }
        public string FieldName { get; set; }
        public override void Execute(IHexTLVFieldParserContext context)
        {
            string recordValue = context.Record.GetFieldValue(FieldName) as string;
            if (!string.IsNullOrEmpty(recordValue))
            {
                DateTime result;
                DateTime.TryParseExact(recordValue, DateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result);
                context.Record.SetFieldValue(this.FieldName, result);
            }
        }
    }
}
