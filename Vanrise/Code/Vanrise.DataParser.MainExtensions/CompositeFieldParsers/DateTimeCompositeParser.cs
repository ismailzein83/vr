using System;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.CompositeFieldParsers
{
    public enum TimeFieldUnit { Seconds = 0, VRTime = 1 }
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
            object dateTimeFieldAsObj = context.Record.GetFieldValue(DateFieldName);
            if (dateTimeFieldAsObj == null)
                return;

            DateTime dateTimeField = (DateTime)dateTimeFieldAsObj;

            double timeFieldValue = default(double);
            if (!string.IsNullOrEmpty(TimeFieldName))
            {
                object timeFieldAsObj = context.Record.GetFieldValue(TimeFieldName);
                if (timeFieldAsObj != null)
                {
                    switch (this.TimeFieldUnit)
                    {
                        case TimeFieldUnit.Seconds:
                            timeFieldValue = Convert.ToDouble(timeFieldAsObj);
                            break;

                        case TimeFieldUnit.VRTime:
                            var timeFieldAsTime = timeFieldAsObj as Vanrise.Entities.Time;
                            timeFieldValue = timeFieldAsTime.TotalSeconds;
                            break;

                        default:
                            throw new NotSupportedException(string.Format("TimeFieldUnit '{0}'", this.TimeFieldUnit));
                    }
                }
            }
            timeFieldValue = SubtractTime ? -timeFieldValue : timeFieldValue;

            DateTime? value = null;
            switch (this.TimeFieldUnit)
            {
                case TimeFieldUnit.Seconds:
                case TimeFieldUnit.VRTime:
                    value = dateTimeField.AddSeconds(timeFieldValue); break;

                default:
                    throw new NotSupportedException(string.Format("TimeFieldUnit '{0}'", this.TimeFieldUnit));
            }

            context.Record.SetFieldValue(FieldName, value);
        }
    }
}