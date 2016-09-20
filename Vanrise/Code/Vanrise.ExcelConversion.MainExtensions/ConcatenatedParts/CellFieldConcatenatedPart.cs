using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Business;
using Vanrise.ExcelConversion.Entities;

namespace Vanrise.ExcelConversion.MainExtensions.ConcatenatedParts
{
    public class CellFieldConcatenatedPart : ConcatenatedPart
    {
        public override Guid ConfigId { get { return new Guid("18280f63-b0f1-42c8-a3cb-d487f17a8236"); } }
        public ExcelConversion.MainExtensions.FieldMappings.CellFieldMapping CellFieldMapping { get; set; }

        public override string GetPartText(IGetConcatenatedPartTextContext context)
        {
            if (this.CellFieldMapping == null)
                throw new NullReferenceException("CellFieldMapping");

            GetFieldValueContext getFieldValueContext = new GetFieldValueContext
            {
                Workbook = context.Workbook,
                Sheet = context.Sheet,
                Row = context.Row
            };

            var cellValue = this.CellFieldMapping.GetFieldValue(getFieldValueContext);
            if (cellValue == null)
                return "";
            else
                return cellValue.ToString();
        }
    }
}
