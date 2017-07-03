using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.CompositeFieldParsers
{
    public class DateTimeCompositeParser : CompositeFieldsParser
    {
        public override Guid ConfigId
        {
            get { return new Guid("068FFEDC-9779-4BCA-AE67-D8A2ACE540D4"); }
        }
        public string FieldName { get; set; }
        public string DateFieldName { get; set; }
        public string TimeFieldName { get; set; }

        public override void Execute(ICompositeFieldsParserContext context)
        {
            DateTime dateTimeField = (DateTime)context.Record.GetFieldValue(DateFieldName);
            TimeSpan timeSpanField = (TimeSpan)context.Record.GetFieldValue(TimeFieldName);
            var value = dateTimeField.AddTicks(timeSpanField.Ticks);
            context.Record.SetFieldValue(FieldName, value);
        }
    }
}
