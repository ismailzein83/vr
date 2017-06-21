using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.Common;
using System.IO;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers
{
    public class SplitByOffSetRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("58E5FB1C-974E-4D5D-BF93-6C2D598FA466"); }
        }

        public int LengthNbOfBytes { get; set; }
        public bool ReverseLengthBytes { get; set; }
        public HexTLVRecordParser RecordParser { get; set; }
        public override void Execute(IHexTLVRecordParserContext context)
        {
            HexTLVHelper.ReadRecordFromStream(context.RecordStream, LengthNbOfBytes, ReverseLengthBytes, (recordValue) =>
            {
                HexTLVHelper.ExecuteRecordParser(RecordParser, new MemoryStream(recordValue.Value), context);
            });
        }
    }
}
