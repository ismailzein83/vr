using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.CompositeFieldParsers
{
    public class TimestampDateTimeCompositeParser : CompositeFieldsParser
    {
        public string FieldName { get; set; }
        public string DateTimeFieldName { get; set; }
        public DateTime DateTimeShift { get; set; }
        public override void Execute(ICompositeFieldsParserContext context)
        {
            DateTime dateTimeField = (DateTime)context.Record.GetFieldValue(DateTimeFieldName);
            TimeSpan timeSpan = dateTimeField - DateTimeShift;
            context.Record.SetFieldValue(FieldName, timeSpan.TotalSeconds);
        }
    }
}
