using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers
{
    public class SplitByBlockRecordParser : HexTLVRecordParserSettings
    {
        public int BlockSize { get; set; }
        public int LengthNbOfBytes { get; set; }
        public bool ReverseLengthBytes { get; set; }
        public int DataLengthIndex { get; set; }
        public int DataLengthBytesLength { get; set; }

        public HexTLVRecordParser RecordParser { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("073FBC75-74F9-4780-971D-C6B122474987"); }
        }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            HexTLVHelper.ReadBlockFromStream(context.RecordStream, BlockSize, (block) =>
            {
                byte[] data = block.Value;
                int headerLength = GetDataLength(data, LengthNbOfBytes, 0);

                byte[] headerData = new byte[headerLength];
                Array.Copy(data, 0, headerData, 0, headerLength);

                int bodyDataLength = GetDataLength(headerData, DataLengthBytesLength, DataLengthIndex);// ParserHelper.GetInt(sizeData, 0, DataLengthBytesLength);

                byte[] bodyData = new byte[bodyDataLength];
                Array.Copy(data, 0, bodyData, 0, bodyDataLength);

                HexTLVHelper.ExecuteRecordParser(RecordParser, new MemoryStream(bodyData), context);

            });
        }

        private int GetDataLength(byte[] data, int lengthOfBytes, int startIndex)
        {
            byte[] lengthData = new byte[lengthOfBytes];
            Array.Copy(data, startIndex, lengthData, 0, lengthOfBytes);
            lengthData = ReverseLengthBytes ? lengthData.Reverse().ToArray() : lengthData;
            int length = ParserHelper.GetInt(lengthData, 0, lengthOfBytes);
            return length;
        }
    }
}
