using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business.HexTLV;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.Entities.HexTLV;
using Vanrise.DataParser.MainExtensions.HexTLV.RecordReaders;
using Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers;

namespace Mediation.Runtime.DataParser
{
    public class DataParserTester
    {
        public void ReadFile()
        {
            byte[] buffer = null;
            var fileStream = new FileStream(@"c:\S-cdr.dat", FileMode.Open, FileAccess.Read);
            buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);

            ReadData(buffer);

        }

        public void ReadData(byte[] data)
        {

            TagRecordReader tagRecordReader = new TagRecordReader();
            Action<string, byte[], Dictionary<string, HexTLVTagType>> onRecordRead = (recordType, recordData, tagTypes) =>
            {
                foreach (var item in tagTypes)
                {
                    TagValueParserExecuteContext tagValueParserExecuteContext = new TagValueParserExecuteContext
                    {
                        Record = new FldDictParsedRecord()
                    };
                    item.Value.ValueParser.Execute(tagValueParserExecuteContext);
                }
            };

            tagRecordReader.RecordTypesByTag = new Dictionary<string, TagRecordType>();
            tagRecordReader.RecordTypesByTag.Add("B4-81", new TagRecordType
            {
                RecordType = "GPRS",
                TagTypes = GetTagTypes()
            });

            HexTLVParserType hexParser = new HexTLVParserType();
            hexParser.RecordReader = tagRecordReader;
            Action<ParsedRecord> onRecordParsed = (parsedRecord) =>
            {

            };
            ParserTypeExecuteContext parserTypeExecuteContext = new ParserTypeExecuteContext(onRecordParsed)
            {
                Input = new DataParserInput
                {
                    Data = data
                }
            };

            hexParser.Execute(parserTypeExecuteContext);

        }

        private Dictionary<string, HexTLVTagType> GetTagTypes()
        {
            Dictionary<string, HexTLVTagType> result = new Dictionary<string, HexTLVTagType>();
            result.Add("9F21", new HexTLVTagType
            {
                ValueParser = new BoolParser { FieldName = "dynamicAddressFlag" }
            });
            result.Add("8A", new HexTLVTagType
            {
                ValueParser = new IntParser { FieldName = "chargingID" }
            });
            result.Add("AB", new HexTLVTagType
            {
                ValueParser = new SequenceParser { TagTypes = GetSquenceTags() }
            });
            return result;
        }

        private Dictionary<string, HexTLVTagType> GetSquenceTags()
        {
            Dictionary<string, HexTLVTagType> result = new Dictionary<string, HexTLVTagType>();
            result.Add("80", new HexTLVTagType
            {
                ValueParser = new IPv4Parser { FieldName = "ggsnAddress" }
            });
            return result;
        }

    }

    #region Classes


    public class ParserTypeExecuteContext : IParserTypeExecuteContext
    {
        Action<ParsedRecord> _OnRecordParsed;
        public ParserTypeExecuteContext(Action<ParsedRecord> onRecordParsed)
        {
            _OnRecordParsed = onRecordParsed;
        }

        public ParsedRecord CreateRecord(string recordType)
        {
            throw new NotImplementedException();
        }

        public IDataParserInput Input
        {
            get;
            set;
        }

        public void OnRecordParsed(ParsedRecord parsedRecord)
        {
            _OnRecordParsed(parsedRecord);
        }
    }

    public class TagValueParserExecuteContext : ITagValueParserExecuteContext
    {

        public ParsedRecord Record
        {
            get;
            set;
        }

        public byte[] TagValue
        {
            get;
            set;
        }
    }

    public class DataParserInput : IDataParserInput
    {
        public byte[] Data
        {
            get;
            set;
        }
    }

    #endregion

}
