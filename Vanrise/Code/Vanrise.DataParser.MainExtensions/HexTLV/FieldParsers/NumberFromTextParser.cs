using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class NumberFromTextParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("4D2CA16E-01FA-4FFD-8DA7-0A0892AB6A65"); }
        }
        public string FieldName { get; set; }
        public NumberType NumberType { get; set; }
        public override void Execute(IHexTLVFieldParserContext context)
        {
            string recordValue = context.Record.GetFieldValue(this.FieldName) as string;

            switch (this.NumberType)
            {
                case NumberType.Decimal:
                    decimal val;
                    decimal.TryParse(recordValue, out val);
                    context.Record.SetFieldValue(this.FieldName, val);
                    break;
                case NumberType.Int:
                    int intVal;
                    int.TryParse(recordValue, out intVal);
                    context.Record.SetFieldValue(this.FieldName, intVal);
                    break;
                case NumberType.BigInt:
                    long longVal;
                    long.TryParse(recordValue, out longVal);
                    context.Record.SetFieldValue(this.FieldName, longVal);
                    break;
            }
        }
    }
}
