using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace Vanrise.ExcelConversion.MainExtensions
{
    public class FixedCellFieldMapping : FieldMapping
    {
        public override Guid ConfigId { get { throw new NotImplementedException(); } }

        public int? SheetIndex { get; set; }

        public int? RowIndex { get; set; }

        public int CellIndex { get; set; }

        public override object GetFieldValue(IGetFieldValueContext context)
        {
            //ExcelWorksheet worksheet = context.Sheet;
            //return worksheet.Rows[RowIndex].Cells[CellIndex].Value;
            
            throw new NotImplementedException();
        }
    }
}
