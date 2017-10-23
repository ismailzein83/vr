using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;
using Vanrise.Common;
namespace Vanrise.ExcelConversion.MainExtensions
{
    public class ConditionalCellFieldMapping : FieldMapping
    {
        public override Guid ConfigId { get { return new Guid("9F9D16F5-9E8B-4D01-8109-6DAABE59623F"); } }

        public string RowFieldName { get; set; }

        //public Dictionary<string, FieldMapping> Choices { get; set; }

        public List<ConditionalCellFieldMappingChoices> Choices { get; set; }

        public override object GetFieldValue(IGetFieldValueContext context)
        {
            var rowFieldValue = context.FieldValueByFieldName[RowFieldName];

            if (rowFieldValue == null)
            {
                var emptyMatched = Choices.FirstOrDefault(x => x.RowFieldValue == null);
                if (emptyMatched != null) return emptyMatched.FieldMappingChoice.GetFieldValue(context);
            }

            var matched = Choices.FindRecord(x => x.RowFieldValue.Equals(rowFieldValue.ToString().Trim(), StringComparison.InvariantCultureIgnoreCase));


            if (matched == null) return null;

            return matched.FieldMappingChoice.GetFieldValue(context);

        }
    }

    public class ConditionalCellFieldMappingChoices
    {
        public string RowFieldValue { get; set; }

        public FieldMapping FieldMappingChoice { get; set; }
    }
}
