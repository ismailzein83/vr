using System;

namespace Vanrise.DataParser.MainExtensions.StringFieldParsers
{
    public class DateFromTextParser : BaseStringParser
    {
        public override Guid ConfigId { get { return new Guid("2ADCD3A8-18E4-4CD0-B4AC-F644E4A72408"); } }
        public string FieldName { get; set; }
        public string DateFormat { get; set; }

        public override void Execute(IBaseStringParserContext context)
        {
            if (string.IsNullOrEmpty(context.FieldValue))
                return;

            DateTime result;
            if (DateTime.TryParseExact(context.FieldValue, DateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
                context.Record.SetFieldValue(this.FieldName, result);
        }
    }
}