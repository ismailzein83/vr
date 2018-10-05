using System;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.CompositeFieldParsers
{
    public class CopyFieldValueCompositeParser : CompositeFieldsParser
    {
        public override Guid ConfigId { get { return new Guid("36C3F6CD-0886-402E-BE97-CD57343EA755"); } }
        public string SourceFieldName { get; set; }
        public string TargetFieldName { get; set; }
        public bool OverrideOnlyIfDefault { get; set; }

        public override void Execute(ICompositeFieldsParserContext context)
        {
            object targetFieldValue = context.Record.GetFieldValue(TargetFieldName);
            if (OverrideOnlyIfDefault && targetFieldValue != null)
                return;

            object sourceFieldValue = context.Record.GetFieldValue(SourceFieldName);
            context.Record.SetFieldValue(TargetFieldName, sourceFieldValue);
        }
    }
}