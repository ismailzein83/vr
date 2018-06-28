using System;
using System.IO;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.HuaweiParser.RecordParsers
{
    public class HeaderRecordParser : BinaryRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("69826C72-B45A-4165-B52A-12BDDE67723F"); } }
        public int HeaderLengthPosition { get; set; }
        public int HeaderBytesLength { get; set; }
        public int FileLengthPosition { get; set; }
        public int FileBytesLength { get; set; }

        public BinaryRecordParser RecordParser { get; set; }

        public override void Execute(IBinaryRecordParserContext context)
        {
            byte[] data = ((MemoryStream)context.RecordStream).ToArray();

            byte[] headerLengthData = new byte[HeaderBytesLength];
            Array.Copy(data, HeaderLengthPosition, headerLengthData, 0, HeaderBytesLength);

            int headerLength = ParserHelper.GetInt(headerLengthData, 0, HeaderBytesLength);
            int dataLength = data.Length - headerLength;
            byte[] parserData = new byte[dataLength];
            Array.Copy(data, headerLength, parserData, 0, dataLength);

            BinaryParserHelper.ExecuteRecordParser(RecordParser, new MemoryStream(parserData), context);
        }
    }
}