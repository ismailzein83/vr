using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class GenericEditorFieldValueContainerRule : GenericEditorConditionalRule
    {
        public override Guid ConfigId { get { return new Guid("A8811BE9-6311-4FB8-9F88-71FD015F639A"); } }
        public List<GenericEditorFieldValueCondition> Conditions { get; set; }
    }

    public class GenericEditorFieldValueCondition
    {
        public string FieldName { get; set; }

        public List<object> FieldValues { get; set; }
    }
}