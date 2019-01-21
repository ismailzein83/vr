using System;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.CompositeFieldParsers
{
    public enum TimeFieldUnit { Seconds = 0 }
    public class DateTimeCompositeParser : CompositeFieldsParser
    {
        public override Guid ConfigId { get { return new Guid("068FFEDC-9779-4BCA-AE67-D8A2ACE540D4"); } }
        public string FieldName { get; set; }
        public string DateFieldName { get; set; }
        public string TimeFieldName { get; set; }
        public TimeFieldUnit TimeFieldUnit { get; set; }
        public bool SubtractTime { get; set; }

        public override void Execute(ICompositeFieldsParserContext context)
        {
            object dateTimeFieldObj = context.Record.GetFieldValue(DateFieldName);
            if (dateTimeFieldObj == null)
                return;

            DateTime dateTimeField = (DateTime)dateTimeFieldObj;

            double timeFieldValue = default(double);
            if (!string.IsNullOrEmpty(TimeFieldName))
            {
                object timeFieldObj = context.Record.GetFieldValue(TimeFieldName);
                if (timeFieldObj != null)
                    timeFieldValue = Convert.ToDouble(timeFieldObj);
            }
            timeFieldValue = SubtractTime ? -timeFieldValue : timeFieldValue;

            DateTime? value = null;
            switch (this.TimeFieldUnit)
            {
                case TimeFieldUnit.Seconds:
                    value = dateTimeField.AddSeconds(timeFieldValue); break;

                default:
                    throw new NotSupportedException(string.Format("TimeFieldUnit '{0}'", this.TimeFieldUnit));
            }

            context.Record.SetFieldValue(FieldName, value);
        }
    }
}