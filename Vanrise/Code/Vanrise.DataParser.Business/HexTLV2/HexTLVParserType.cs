using System;
using System.Collections.Generic;
using System.IO;
using Vanrise.Common;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.Business
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


            public ParsedRecord CreateRecord(string recordType, HashSet<string> tempFieldNames)
            {
                return _parserTypeContext.CreateRecord(recordType, tempFieldNames);
            }
        }

        #endregion
    }
}
