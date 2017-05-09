using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vanrise.Common;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV;
using Vanrise.DataParser.Business;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordReaders
{
    public class TagRecordReader : RecordReader
    {
        public override Guid ConfigId
        {
            get { return new Guid("1F558330-45F4-4C00-8E5C-4B51ED8F6349"); }
        }
        public Dictionary<string, TagRecordType> RecordTypesByTag { get; set; }
        public int NumberOfBytesToSkip { get; set; }
        public override void Execute(IRecordReaderExecuteContext context)
        {
            MemoryStream stream = new MemoryStream(context.Data);
            int position = 0;
            while (position < stream.Length)
            {
                string tag = ParserHelper.GetStringValue(stream, 1);
                position++;
                stream.SkipBytes(NumberOfBytesToSkip);
                position += NumberOfBytesToSkip;
                int recordLength = ParserHelper.GetIntValue(stream, 1);
                position += recordLength + 1;
                TagRecordType tagRecordType;
                byte[] recordData = new byte[recordLength];
                stream.Read(recordData, 0, recordLength);

                if (RecordTypesByTag.TryGetValue(tag, out tagRecordType))
                {
                    context.OnRecordRead(tagRecordType.RecordType, recordData, tagRecordType.TagTypes);
                }
            }
        }
    }

    public class TagRecordType
    {
        public string RecordType { get; set; }

        public Dictionary<string, HexTLVTagType> TagTypes { get; set; }
    }


}
