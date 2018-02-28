using System;
using System.Collections.Generic;
using System.IO;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using System.Linq;

namespace Vanrise.DataParser.MainExtensions.AlcatelParsers.RecordParsers
{
    public class HeaderRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("BB5EE31F-DB10-4480-9A25-9919B71CCB57"); } }
        public int HeaderByteLength { get; set; }
        public string RecordStartingTag { get; set; }
        public HexTLVRecordParser RecordParser { get; set; }
        public List<string> TagsToIgnore { get; set; }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            List<byte[]> tagsToIgnoreBytes = null;
            int? tagToIgnoreBytesLength = null;
            if (this.TagsToIgnore != null)
            {
                tagsToIgnoreBytes = new List<byte[]>();
                foreach (string tagToIgnore in this.TagsToIgnore)
                {
                    byte[] tagToIgnoreBytes = ParserHelper.StringToByteArray(tagToIgnore);
                    tagsToIgnoreBytes.Add(tagToIgnoreBytes);
                    tagToIgnoreBytesLength = tagToIgnoreBytes.Length;
                }
            }

            HexTLVHelper.ReadBlockFromStream(context.RecordStream, HeaderByteLength, (header) =>
            {
                string headerTagHex = string.Concat(ParserHelper.GetHexFromByte(header.Value[0]), ParserHelper.GetHexFromByte(header.Value[1]));
                if (string.Compare(this.RecordStartingTag, headerTagHex, true) == 0)
                {
                    byte[] yearBytes = new byte[1] { header.Value[5] };

                    int year = Convert.ToInt32(ParserHelper.GetBCDNumber(yearBytes, false, false));
                    year = 2000 + year;
                    context.SetGlobalVariable("Year", year);

                    MemoryStream ms = new MemoryStream();
                    context.RecordStream.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    List<byte> validBytes = new List<byte>();
                    if (tagsToIgnoreBytes != null)
                    {
                        HexTLVHelper.ReadBlockFromStream(ms, tagToIgnoreBytesLength.Value, (content) =>
                        {
                            if (tagsToIgnoreBytes.FirstOrDefault(itm => Vanrise.Common.Utilities.AreEquals(content.Value, itm)) == null)
                            {
                                validBytes.Add(content.Value[0]);
                                ms.Seek(ms.Position - tagToIgnoreBytesLength.Value + 1, SeekOrigin.Begin);
                            }
                        });
                        HexTLVHelper.ExecuteRecordParser(RecordParser, new MemoryStream(validBytes.ToArray()), context);
                    }
                    else
                    {
                        HexTLVHelper.ExecuteRecordParser(RecordParser, ms, context);
                    }
                }
                else
                {
                    context.RecordStream.Seek(0, SeekOrigin.End);
                }
            }, true);
        }
    }
}