using System;
using System.Collections.Generic;
using System.IO;
using Vanrise.DataParser.Entities;
using Vanrise.Common;

namespace Vanrise.DataParser.Business
{
    public class HexTLVParserHelper
    {
        public static void ExecuteFieldParsers(BinaryFieldParserCollection fieldParsers, ParsedRecord parsedRecord, Stream recordStream)
        {
            fieldParsers.ThrowIfNull("fieldParsers");
            fieldParsers.FieldParsersByTag.ThrowIfNull("fieldParsers.FieldParsersByTag");
            ReadTagsFromStream(recordStream,
                (tagValue) =>
                {
                    BinaryFieldParser fldParser;
                    if (fieldParsers.FieldParsersByTag.TryGetValue(tagValue.Tag, out fldParser))
                    {
                        var fieldParserContext = new BinaryFieldParserContext { Record = parsedRecord, FieldValue = tagValue.Value };
                        fldParser.Settings.Execute(fieldParserContext);
                    }
                });
        }

        public static void ReadTagsFromStream(Stream stream, Action<HexTLVTagValue> onTagValueRead)
        {
            byte[] rawData = new byte[stream.Length];
            stream.Read(rawData, 0, (int)stream.Length);
            List<HexTLVTagValue> tags = new List<HexTLVTagValue>();

            for (int i = 0, start = 0; i < rawData.Length; start = i)
            {
                // parse Tag
                bool constructedTlv = (rawData[i] & 0x20) != 0;
                bool moreBytes = (rawData[i] & 0x1F) == 0x1F;
                while (moreBytes && (rawData[++i] & 0x80) != 0) ;
                i++;

                int tag = ParserHelper.GetInt(rawData, start, i - start);
                if (tag == 0 || tag == 255)
                    continue;
                // parse Length
                bool multiByteLength = (rawData[i] & 0x80) != 0;

                int length = multiByteLength ? ParserHelper.GetInt(rawData, i + 1, rawData[i] & 0x1F) : rawData[i];
                i = multiByteLength ? i + (rawData[i] & 0x1F) + 1 : i + 1;

                // fill data
                byte[] result = new byte[length];

                Array.Copy(rawData, i, result, 0, length);
                HexTLVTagValue tagValue = new HexTLVTagValue
                {
                    Value = result,
                    Length = length,
                    Tag = tag.ToString("X2")
                };
                tags.Add(tagValue);
                i += length;
                onTagValueRead(tagValue);
            }
        }
    }
}
