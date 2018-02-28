using System;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.CompositeFieldParsers
{
    public class DateFromDayNumberCompositeParser : CompositeFieldsParser
    {
        public override Guid ConfigId { get { return new Guid("D0251EE2-1FB9-4B7C-99E9-CAE8C2A90705"); } }
        public string FieldName { get; set; }
        public string YearFieldName { get; set; }
        public string DayNumberFieldName { get; set; }

        public override void Execute(ICompositeFieldsParserContext context)
        {
            int year = Convert.ToInt32(context.Record.GetFieldValue(YearFieldName));
            int dayNumber = Convert.ToInt32(context.Record.GetFieldValue(DayNumberFieldName));

            DateTime date = new DateTime(year, 1, 1).AddDays(dayNumber - 1);

            context.Record.SetFieldValue(FieldName, date);
        }
    }
}
