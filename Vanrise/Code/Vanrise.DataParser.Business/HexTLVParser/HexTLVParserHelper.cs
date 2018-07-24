using System;
using System.Collections.Generic;
using System.IO;
using Vanrise.DataParser.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.DataParser.Business
{
    public class HexTLVParserHelper
    {
        public static void ExecuteFieldParsers(BinaryFieldParserCollection fieldParsers, ParsedRecord parsedRecord, Stream recordStream)
        {
            fieldParsers.ThrowIfNull("fieldParsers");
            fieldParsers.FieldParsersByTag.ThrowIfNull("fieldParsers.FieldParsersByTag");
            ReadTagsFromStream(recordStream, (tagValue) =>
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
                //Parsing Tag
                bool constructedTlv = (rawData[i] & 0x20) != 0;
                bool moreBytes = (rawData[i] & 0x1F) == 0x1F;
                while (moreBytes && (rawData[++i] & 0x80) != 0) ;
                i++;

                int tag = ParserHelper.GetInt(rawData, start, i - start);
                if (tag == 0 || tag == 255)
                    continue;

                //Parsing Length
                int length;
                bool hasFixedLength = rawData[i] != 0x80;
                if (hasFixedLength)
                {
                    bool multiByteLength = (rawData[i] & 0x80) != 0;
                    length = multiByteLength ? ParserHelper.GetInt(rawData, i + 1, rawData[i] & 0x1F) : rawData[i];
                    i = multiByteLength ? i + (rawData[i] & 0x1F) + 1 : i + 1;
                }
                else
                {
                    i++;
                    length = GetTagValueLength(rawData, i, tag);
                }

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

        private static int GetTagValueLength(byte[] rawData, int startingIndex, int parentTag)
        {
            int numberOfByte80 = 1;

            for (int currentIndex = startingIndex, start = startingIndex; currentIndex < rawData.Length; start = currentIndex)
            {
                //Parsing Tag
                bool moreBytes = (rawData[currentIndex] & 0x1F) == 0x1F;
                while (moreBytes && (rawData[++currentIndex] & 0x80) != 0) ;
                currentIndex++;

                int tag = ParserHelper.GetInt(rawData, start, currentIndex - start);
                if (tag == 255)
                    continue;

                //Parsing Length
                bool hasFixedLength = rawData[currentIndex] != 0x80;
                bool multiByteLength = (rawData[currentIndex] & 0x80) != 0;
                int length = multiByteLength ? ParserHelper.GetInt(rawData, currentIndex + 1, rawData[currentIndex] & 0x1F) : rawData[currentIndex];
                currentIndex = multiByteLength ? currentIndex + (rawData[currentIndex] & 0x1F) + 1 : currentIndex + 1;
                currentIndex += length;

                if (length != 0 || (tag != 0 && hasFixedLength))
                    continue;

                //we are here in two cases:
                //tag = !00 and length = 80
                //tag = 00  and length = 00
                if (tag != 0)
                {
                    numberOfByte80++;
                }
                else
                {
                    numberOfByte80--;
                    if (numberOfByte80 == 0)
                        return currentIndex - startingIndex;
                }
            }

            throw new DataIntegrityValidationException(string.Format("Unknow Length for Tag: '{0}'", parentTag.ToString("X")));
        }
    }
}
