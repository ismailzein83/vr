using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers
{
    public class PositionedFieldsRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("301B945E-765F-4D90-952E-D86DA4AE4040"); }
        }

        public string RecordType { get; set; }

        public List<PositionedFieldParser> FieldParsers { get; set; }  
        
        public List<ParsedRecordFieldConstantValue> FieldConstantValues { get; set; }

        public List<CompositeFieldsParser> CompositeFieldsParsers { get; set; }

        public HashSet<string> TempFieldsNames { get; set; }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class PositionedFieldParser
    {
        public int Position { get; set; }

        public int Length { get; set; }

        public HexTLVFieldParser FieldParser { get; set; }
    }
}
