using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;
using Vanrise.Entities;
using Vanrise.Common.Business;
namespace Vanrise.ExcelConversion.MainExtensions.FieldMappings
{
    public class CellFieldMapping : FieldMapping
    {
        public int? SheetIndex { get; set; }

        public int? RowIndex { get; set; }

        public int CellIndex { get; set; }

        public TextManipulationSettings ManipulationSettings { get; set; }

        public override object GetFieldValue(IGetFieldValueContext context)
        {
            Aspose.Cells.Row row = context.Row;
            if (row == null)
            {
                Aspose.Cells.Worksheet worksheet = context.Sheet;
                if (worksheet == null)
                {
                    if (context.Workbook == null)
                        throw new ArgumentNullException("context.Workbook");
                    if (!this.SheetIndex.HasValue)
                        throw new NullReferenceException("SheetIndex");
                    if (context.Workbook.Worksheets.Count <= this.SheetIndex.Value)
                        throw new Exception(String.Format("SheetIndex '{0}' is greater than max index in the workbook", this.SheetIndex.Value));
                    worksheet = context.Workbook.Worksheets[this.SheetIndex.Value];
                }
                if (!this.RowIndex.HasValue)
                    throw new NullReferenceException("RowIndex");
                if (worksheet.Cells.Rows.Count <= this.RowIndex.Value)
                    throw new Exception(String.Format("RowIndex '{0}' is greater than max index in the worksheet", this.RowIndex.Value));
                row = worksheet.Cells.Rows[this.RowIndex.Value];
            }
            var cell = row.GetCellOrNull(this.CellIndex);

            if (this.ManipulationSettings != null && this.ManipulationSettings.Actions != null)
            {
                TextManipulationActionContext textManipulationActionContext = new TextManipulationActionContext();
                TextManipulationTarget target = new TextManipulationTarget
                {
                    TextValue = cell != null ? cell.Value.ToString() : null
                };
                foreach(var action in this.ManipulationSettings.Actions)
                {
                    action.Execute(textManipulationActionContext, target);
                }
                return target.TextValue;
            }
            if (cell != null)
                return cell.Value;
            else
                return null;
        }
    }
}
