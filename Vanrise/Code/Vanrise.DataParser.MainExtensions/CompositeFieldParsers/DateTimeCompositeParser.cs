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
        public string SecondsToAddFieldName { get; set; }
        public bool SubtractTime { get; set; }

        public override void Execute(ICompositeFieldsParserContext context)
        {
            DateTime dateTimeField = (DateTime)context.Record.GetFieldValue(DateFieldName);
            double timeSpanField = (double)context.Record.GetFieldValue(TimeFieldName);
            timeSpanField = SubtractTime ? -timeSpanField : timeSpanField;
            var value = dateTimeField.AddSeconds(timeSpanField);
            if (!string.IsNullOrEmpty(this.SecondsToAddFieldName))
            {
                int seconds = (int)context.Record.GetFieldValue(SecondsToAddFieldName);
                value.AddSeconds(seconds);
            }
            context.Record.SetFieldValue(FieldName, value);
        }
    }
}
