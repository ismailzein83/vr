using System;
using System.Collections.Generic;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericEditorFieldValueConditionalRule : GenericEditorConditionalRule
    {
        public override Guid ConfigId { get { return new Guid("A8811BE9-6311-4FB8-9F88-71FD015F639A"); } }

        public override string ActionName { get { return "FieldValueConditionalRuleAction"; } }

        public List<GenericEditorFieldValueCondition> Conditions { get; set; }
    }

    public class GenericEditorFieldValueCondition
    {
        public string FieldName { get; set; }

        public List<object> FieldValues { get; set; }
    }
}