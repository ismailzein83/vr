using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.Business.HexTLV
{
    public class HexTLVParserType : ParserTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("DF8E951E-9D65-4F49-BC92-18F6E159D7DF"); }
        }
        public RecordReader RecordReader { get; set; }

        public override void Execute(IParserTypeExecuteContext context)
        {
            Action<string, byte[], Dictionary<string, HexTLVTagType>> onRecordRead = (recordType, recordData, tagTypes) =>
            {
                MemoryStream stream = new MemoryStream(recordData);
                int position = 0;
                ParsedRecord parsedRecord = context.CreateRecord(recordType);
                while (position < stream.Length)
                {
                    HexTLVTagType hexTLVTagType;
                    string tag = ParserHelper.GetStringValue(stream, 1);
                    position++;
                    int recordLength = ParserHelper.GetIntValue(stream, 1);
                    position += recordLength;
                    byte[] fieldData = new byte[recordLength];
                    stream.Read(fieldData, 0, recordLength);
                    if (tagTypes.TryGetValue(tag, out hexTLVTagType))
                    {
                        TagValueParserExecuteContext tagValueParserExecuteContext = new TagValueParserExecuteContext
                        {
                            TagValue = fieldData,
                            Record = parsedRecord
                        };
                        hexTLVTagType.ValueParser.Execute(tagValueParserExecuteContext);
                    }
                }
            };
            RecordReaderExecuteContext recordReaderExecuteContext = new RecordReaderExecuteContext(onRecordRead)
            {
                Data = context.Input.Data

            };
            RecordReader.Execute(recordReaderExecuteContext);
        }


        class RecordReaderExecuteContext : IRecordReaderExecuteContext
        {
            Action<string, byte[], Dictionary<string, HexTLVTagType>> _OnRecordRead;
            public RecordReaderExecuteContext(Action<string, byte[], Dictionary<string, HexTLVTagType>> onRecordRead)
            {
                _OnRecordRead = onRecordRead;
            }

            public byte[] Data
            {
                get;
                set;
            }

            public void OnRecordRead(string recordType, byte[] recordData, Dictionary<string, HexTLVTagType> tagTypes)
            {
                _OnRecordRead(recordType, recordData, tagTypes);
            }
        }
        class TagValueParserExecuteContext : ITagValueParserExecuteContext
        {
            public byte[] TagValue
            {
                get;
                set;
            }

            public ParsedRecord Record
            {
                get;
                set;
            }
        }

    }
}
