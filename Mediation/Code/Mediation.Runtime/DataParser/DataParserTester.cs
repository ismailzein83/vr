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

            ReadData(buffer.ToList());

        }

        public void ReadData(List<byte> data)
        {

            TagRecordReader tagRecordReader = new TagRecordReader();
            Action<string, List<byte>, Dictionary<string, HexTLVTagType>> onRecordRead = (recordType, recordData, tagTypes) =>
            {

            };
            RecordReaderExecuteContext context = new RecordReaderExecuteContext(onRecordRead)
            {
                Data = data
            };

            tagRecordReader.TagRecordTypes = new List<TagRecordType>();
            tagRecordReader.TagRecordTypes.Add(new TagRecordType
            {
                RecordType = "GPRS",
                Tag = "B4",
                TagTypes = GetTagTypes()
            });

            HexTLVParserType hexParser = new HexTLVParserType();
            hexParser.RecordReader = tagRecordReader;
            Action<ParsedRecord> onRecordParsed = (parsedRecord) =>
            {

            };
            ParserTypeExecuteContext parserTypeExecuteContext = new ParserTypeExecuteContext(onRecordParsed)
            {
                Input = new DataParserInput()
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
                ValueParser = new IPParser { FieldName = "ggsnAddress" }
            });
            return result;
        }

    }

    #region Classes

    public class RecordReaderExecuteContext : IRecordReaderExecuteContext
    {
        Action<string, List<byte>, Dictionary<string, HexTLVTagType>> _OnRecordRead;
        public RecordReaderExecuteContext(Action<string, List<byte>, Dictionary<string, HexTLVTagType>> onRecordRead)
        {
            _OnRecordRead = onRecordRead;
        }

        public List<byte> Data
        {
            get;
            set;
        }

        public void OnRecordRead(string recordType, List<byte> recordData, Dictionary<string, HexTLVTagType> tagTypes)
        {
            _OnRecordRead(recordType, recordData, tagTypes);
        }
    }

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

    public class DataParserInput : IDataParserInput
    {
    }

    #endregion

}
