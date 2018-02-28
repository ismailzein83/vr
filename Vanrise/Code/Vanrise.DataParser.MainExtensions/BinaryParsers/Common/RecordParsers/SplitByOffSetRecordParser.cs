using System;
using System.IO;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers
{
    public class SplitByOffSetRecordParser : BinaryRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("58E5FB1C-974E-4D5D-BF93-6C2D598FA466"); } }
        public int LengthNbOfBytes { get; set; }
        public bool ReverseLengthBytes { get; set; }
        public BinaryRecordParser RecordParser { get; set; }

        public override void Execute(IBinaryRecordParserContext context)
        {
            BinaryParserHelper.ReadRecordFromStream(context.RecordStream, LengthNbOfBytes, ReverseLengthBytes, (recordValue) =>
            {
                BinaryParserHelper.ExecuteRecordParser(RecordParser, new MemoryStream(recordValue.Value), context);
            });
        }
    }
}
