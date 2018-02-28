using System;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.StringFieldParsers
{
    public class NumberFromTextParser : BaseStringParser
    {
        public override Guid ConfigId { get { return new Guid("4D2CA16E-01FA-4FFD-8DA7-0A0892AB6A65"); } }
        public string FieldName { get; set; }
        public NumberType NumberType { get; set; }

        public override void Execute(IBaseStringParserContext context)
        {
            if (string.IsNullOrEmpty(context.FieldValue))
                return;

            switch (this.NumberType)
            {
                case NumberType.Decimal:
                    decimal val;
                    if (decimal.TryParse(context.FieldValue, out val))
                        context.Record.SetFieldValue(this.FieldName, val);
                    break;

                case NumberType.Int:
                    int intVal;
                    if (int.TryParse(context.FieldValue, out intVal))
                        context.Record.SetFieldValue(this.FieldName, intVal);
                    break;

                case NumberType.BigInt:
                    long longVal;
                    if (long.TryParse(context.FieldValue, out longVal))
                        context.Record.SetFieldValue(this.FieldName, longVal);
                    break;
            }
        }
    }
}
