using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;
using Vanrise.Common;
using System.IO;
using Vanrise.DataParser.Entities.HexTLV2;

namespace Vanrise.DataParser.Business.HexTLV2
{
    public class HexTLVParserType : ParserTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("92CA6F6F-8901-4C86-9540-EFA2941D25E2"); }
        }

        public HexTLVRecordParserSettings RecordParser { get; set; }

        public Dictionary<Guid, HexTLVRecordParser> RecordParserTemplates { get; set; }

        public override void Execute(IParserTypeExecuteContext context)
        {
            StreamDataParserInput streamDataParserInput = context.Input.CastWithValidate<StreamDataParserInput>("context.Input");
            streamDataParserInput.Stream.ThrowIfNull("streamDataParserInput.Stream");
            this.RecordParser.ThrowIfNull("this.RecordParser");
            var recordParserContext = new HexTLVRecordParserContext(streamDataParserInput.Stream, context, this.RecordParserTemplates);
            this.RecordParser.Execute(recordParserContext);
        }

        #region Private Classes
        
        private class HexTLVRecordParserContext : IHexTLVRecordParserContext
        {
            Stream _recordStream;
            IParserTypeExecuteContext _parserTypeContext;
            Dictionary<Guid, HexTLVRecordParser> _recordParserTemplates;

            public HexTLVRecordParserContext(Stream recordStream, IParserTypeExecuteContext parserTypeContext, Dictionary<Guid, HexTLVRecordParser> recordParserTemplates)
            {
                recordStream.ThrowIfNull("recordData");
                parserTypeContext.ThrowIfNull("parserTypeContext");
                _recordStream = recordStream;
                _parserTypeContext = parserTypeContext;
                _recordParserTemplates = recordParserTemplates;
            }

            public Stream RecordStream
            {
                get { return _recordStream; }
            }

            public ParsedRecord CreateRecord(string recordType)
            {
                return _parserTypeContext.CreateRecord(recordType);
            }

            public void OnRecordParsed(ParsedRecord parsedRecord)
            {
                _parserTypeContext.OnRecordParsed(parsedRecord);
            }


            public HexTLVRecordParser GetParserTemplate(Guid templateId)
            {
                this._recordParserTemplates.ThrowIfNull("_recordParserTemplates");
                HexTLVRecordParser recordParser = this._recordParserTemplates.GetRecord(templateId);
                recordParser.ThrowIfNull("recordParser", templateId);
                return recordParser;
            }
        }
        
        #endregion
    }
}
