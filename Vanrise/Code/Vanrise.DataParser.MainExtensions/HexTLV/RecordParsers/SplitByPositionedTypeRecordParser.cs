using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers
{
    public class SplitByPositionedTypeRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("2FCEDED6-C422-4FB2-92C7-3ED32D6EEF78"); }
        }

        public int RecordTypePosition { get; set; }

        public int RecordTypeLength { get; set; }

        public Dictionary<string, HexTLVRecordParser> SubRecordsParsersByRecordType { get; set; }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            throw new NotImplementedException();
        }
    }
}
