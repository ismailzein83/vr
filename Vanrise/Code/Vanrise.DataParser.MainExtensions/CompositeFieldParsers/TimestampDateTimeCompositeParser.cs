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
        public override Guid ConfigId { get { return new Guid("51D08426-F968-4A8C-B52B-90FF55E8630E"); } }
        public string FieldName { get; set; }
        public string DateTimeFieldName { get; set; }
        public DateTime DateTimeShift { get; set; }

        public override void Execute(ICompositeFieldsParserContext context)
        {
            try
            {
                TimeSpan timeSpan = new TimeSpan();
                DateTime? dateTimeField = (DateTime?)context.Record.GetFieldValue(DateTimeFieldName);
                if (dateTimeField.HasValue && dateTimeField.Value != DateTime.MinValue)
                {
                    timeSpan = dateTimeField.Value - DateTimeShift;
                }
                context.Record.SetFieldValue(FieldName, timeSpan.TotalSeconds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
