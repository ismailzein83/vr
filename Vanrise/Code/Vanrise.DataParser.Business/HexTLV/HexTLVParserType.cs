using System;
using System.Collections.Generic;
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
            get { throw new NotImplementedException(); }
        }
        public RecordReader RecordReader { get; set; }

        public override void Execute(IParserTypeExecuteContext context)
        {
            Action<string, byte[], Dictionary<string, HexTLVTagType>> onRecordRead = (recordType, recordData, tagTypes) =>
            {

            };
            RecordReaderExecuteContext recordReaderExecuteContext = new RecordReaderExecuteContext(onRecordRead)
            {
                Data = context.Input.Data
            };
            RecordReader.Execute(recordReaderExecuteContext);
        }
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


}
