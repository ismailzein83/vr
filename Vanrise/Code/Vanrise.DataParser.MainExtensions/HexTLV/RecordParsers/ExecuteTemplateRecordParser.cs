using System;
using Vanrise.Common;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers
{
    public class ExecuteTemplateRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("D1E25FB3-4F9C-4FB4-BD92-F9D16064279D"); }
        }

        public Guid RecordParserTemplateId { get; set; }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            var recordParser = context.GetParserTemplate(this.RecordParserTemplateId);
            recordParser.ThrowIfNull("recordParser", this.RecordParserTemplateId);
            recordParser.Settings.ThrowIfNull("recordParser.Settings", this.RecordParserTemplateId);
            recordParser.Settings.Execute(context);
        }
    }
}
