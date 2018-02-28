﻿using System;
using System.IO;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.NokiaSiemensParsers.RecordParsers
{
    public class HeaderRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("67B49D8E-0285-4926-9AFB-8A63F6AF8DD6"); } }
        public int HeaderLength { get; set; }
        public int RecordLengthIndex { get; set; }
        public int RecordLengthByteLength { get; set; }
        public HexTLVRecordParser PackageRecordParser { get; set; }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            HexTLVHelper.ReadBlockFromStream(context.RecordStream, 1, (headerTag) =>
            {
                int headerTagAsInt = ParserHelper.GetInt(headerTag.Value, 0, 1);
                switch (headerTagAsInt)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3: HexTLVHelper.ReadBlockFromStream(context.RecordStream, 31, (record) => { }, true); break;
                    case 128: break;

                    case 129:
                        HexTLVHelper.ReadBlockFromStream(context.RecordStream, 1, (recordByteLength) =>
                        {
                            int recordLength = ParserHelper.GetInt(recordByteLength.Value, 0, 1);
                            HexTLVHelper.ReadBlockFromStream(context.RecordStream, recordLength - 2, (record) =>
                            {
                            }, true);
                        }, true);
                        break;

                    case 132:
                        HexTLVHelper.ReadBlockFromStream(context.RecordStream, HeaderLength - 1, (headerWithoutTag) =>
                        {
                            int recordLength = ParserHelper.GetInt(headerWithoutTag.Value, 0, RecordLengthByteLength);

                            HexTLVHelper.ReadBlockFromStream(context.RecordStream, recordLength - HeaderLength, (record) =>
                            {
                                HexTLVHelper.ExecuteRecordParser(PackageRecordParser, new MemoryStream(record.Value), context);
                            }, true);
                        }, true);
                        break;

                    default: throw new NotSupportedException(string.Format("Invalid Tag Header value: {0}", headerTagAsInt));
                }
            });
        }
    }
}