using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace Vanrise.ExcelConversion.MainExtensions
{
    public class ConditionalCellFieldMapping : FieldMapping
    {
        public override Guid ConfigId { get { throw new NotImplementedException(); } }

        public string RowFieldName { get; set; }

        public Dictionary<string, FieldMapping> Choices { get; set; }

        //public List<ConditionalCellFieldMappingChoices> Choices { get; set; }

        public override object GetFieldValue(IGetFieldValueContext context)
        {
            //var rowFieldValue = context.FieldValueByFieldName[RowFieldName];
            //FieldMapping selectedFieldMapping = Choices[rowFieldValue.ToString()];
            
            //return selectedFieldMapping.GetFieldValue(context);

            throw new NotImplementedException();
        }
    }

    //public class ConditionalCellFieldMappingChoices
    //{
    //    public string RowFieldValue { get; set; }

    //    public FieldMapping FieldMappingChoice { get; set; }
    //}
}
