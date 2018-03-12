using System;
using System.Collections.Generic;
using System.IO;
using Vanrise.Common;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.Business
{
    public class BinaryParserType : ParserTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("92CA6F6F-8901-4C86-9540-EFA2941D25E2"); } }

        public BinaryRecordParserSettings RecordParser { get; set; }

        public Dictionary<Guid, BinaryRecordParser> RecordParserTemplates { get; set; }

        public override void Execute(IParserTypeExecuteContext context)
        {
            StreamDataParserInput streamDataParserInput = context.Input.CastWithValidate<StreamDataParserInput>("context.Input");
            streamDataParserInput.Stream.ThrowIfNull("streamDataParserInput.Stream");
            this.RecordParser.ThrowIfNull("this.RecordParser");

            var recordParserContext = new BinaryRecordParserContext(streamDataParserInput.Stream, streamDataParserInput.FileName, streamDataParserInput.DataSourceId, context, this.RecordParserTemplates);
            this.RecordParser.Execute(recordParserContext);
        }

        #region Private Classes

        private class BinaryRecordParserContext : IBinaryRecordParserContext
        {
            Stream _recordStream;
            string _fileName;
            Guid _dataSourceId;
            IParserTypeExecuteContext _parserTypeContext;
            Dictionary<Guid, BinaryRecordParser> _recordParserTemplates;

            public Stream RecordStream { get { return _recordStream; } }

            public string FileName { get { return _fileName; } }

            public Guid DataSourceId { get { return _dataSourceId; } }


            public BinaryRecordParserContext(Stream recordStream, string fileName, Guid dataSourceId, IParserTypeExecuteContext parserTypeContext, Dictionary<Guid, BinaryRecordParser> recordParserTemplates)
            {
                recordStream.ThrowIfNull("recordData");
                parserTypeContext.ThrowIfNull("parserTypeContext");
                _parserTypeContext = parserTypeContext;
                _recordParserTemplates = recordParserTemplates;
                _recordStream = recordStream;
                _fileName = fileName;
                _dataSourceId = dataSourceId;
            }

            public void OnRecordParsed(ParsedRecord parsedRecord)
            {
                _parserTypeContext.OnRecordParsed(parsedRecord);
            }

            public BinaryRecordParser GetParserTemplate(Guid templateId)
            {
                this._recordParserTemplates.ThrowIfNull("_recordParserTemplates");
                BinaryRecordParser recordParser = this._recordParserTemplates.GetRecord(templateId);
                recordParser.ThrowIfNull("recordParser", templateId);
                return recordParser;
            }

            public ParsedRecord CreateRecord(string recordType, HashSet<string> tempFieldNames)
            {
                return _parserTypeContext.CreateRecord(recordType, tempFieldNames);
            }

            public void SetGlobalVariable(string variableName, dynamic value)
            {
                _parserTypeContext.SetGlobalVariable(variableName, value);
            }

            public Dictionary<string, dynamic> GetGlobalVariables()
            {
                return _parserTypeContext.GetGlobalVariables();
            }
        }

        #endregion
    }
}