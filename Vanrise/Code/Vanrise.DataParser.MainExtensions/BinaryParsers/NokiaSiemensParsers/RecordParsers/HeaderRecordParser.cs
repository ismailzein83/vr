﻿using System;
using System.IO;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.RecordParsers
{
    public class HeaderRecordParser : BinaryRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("67B49D8E-0285-4926-9AFB-8A63F6AF8DD6"); } }
        public int HeaderLength { get; set; }
        public int HeaderTagLength { get; set; }
        public int RecordLengthByteLength { get; set; }
        public BinaryRecordParser PackageRecordParser { get; set; }

        public override void Execute(IBinaryRecordParserContext context)
        {
            BinaryParserHelper.ReadBlockFromStream(context.RecordStream, HeaderTagLength, (headerTag) =>
            {
                int headerTagAsInt = ParserHelper.GetInt(headerTag.Value, 0, 1);
                switch (headerTagAsInt)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3: BinaryParserHelper.ReadBlockFromStream(context.RecordStream, 31, (record) => { }, true); break;
                    case 128: break;

                    case 129:
                        BinaryParserHelper.ReadBlockFromStream(context.RecordStream, RecordLengthByteLength, (recordByteLength) =>
                        {
                            int recordLength = ParserHelper.HexToInt32(ParserHelper.ByteArrayToString(recordByteLength.Value, true));
                            
                            BinaryParserHelper.ReadBlockFromStream(context.RecordStream, recordLength - (HeaderTagLength + RecordLengthByteLength), (record) =>
                            {
                            }, true);
                        }, true);
                        break;

                    case 132:
                        BinaryParserHelper.ReadBlockFromStream(context.RecordStream, HeaderLength - HeaderTagLength, (headerWithoutTag) =>
                        {
                            byte[] bytes = new byte[RecordLengthByteLength];
                            Array.Copy(headerWithoutTag.Value, 0, bytes, 0, RecordLengthByteLength);
                            int recordLength = ParserHelper.HexToInt32(ParserHelper.ByteArrayToString(bytes, true));

                            BinaryParserHelper.ReadBlockFromStream(context.RecordStream, recordLength - HeaderLength, (record) =>
                            {
                                BinaryParserHelper.ExecuteRecordParser(PackageRecordParser, new MemoryStream(record.Value), context);
                            }, true);
                        }, true);
                        break;

                    default: throw new NotSupportedException(string.Format("Invalid Tag Header value: {0}", headerTagAsInt));
                }
            });
        }
    }
}