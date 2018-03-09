using System;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.CompositeFieldParsers
{
    public class DateTimeCompositeParser : CompositeFieldsParser
    {
        public override Guid ConfigId { get { return new Guid("068FFEDC-9779-4BCA-AE67-D8A2ACE540D4"); } }
        public string FieldName { get; set; }
        public string DateFieldName { get; set; }
        public string TimeFieldName { get; set; }
        public string SecondsToAddFieldName { get; set; }
        public bool SubtractTime { get; set; }

        public override void Execute(ICompositeFieldsParserContext context)
        {
            object dateTimeFieldObj = context.Record.GetFieldValue(DateFieldName);
            if (dateTimeFieldObj == null)
                return;

            DateTime dateTimeField = (DateTime)dateTimeFieldObj;

            double timeSpanField = default(double);
            if (!string.IsNullOrEmpty(TimeFieldName))
            {
                object timeFieldObj = context.Record.GetFieldValue(TimeFieldName);
                if (timeFieldObj != null)
                    timeSpanField = (double)timeFieldObj;
            }

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