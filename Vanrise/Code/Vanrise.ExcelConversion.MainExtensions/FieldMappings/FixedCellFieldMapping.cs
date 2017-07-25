using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.ExcelConversion.Entities;

namespace Vanrise.ExcelConversion.MainExtensions
{
    public class FixedCellFieldMapping : FieldMapping
    {
        public override Guid ConfigId { get { return new Guid("CB6F97C0-8112-4954-9C80-B02AB1A1C69C"); } }

        public int? SheetIndex { get; set; }

        public int RowIndex { get; set; }

        public int CellIndex { get; set; }

        public TextManipulationSettings ManipulationSettings { get; set; }
        public override object GetFieldValue(IGetFieldValueContext context)
        {
            Aspose.Cells.Worksheet worksheet = context.Sheet;
            var cell = worksheet.Cells[RowIndex, CellIndex];
            if (this.ManipulationSettings != null && this.ManipulationSettings.Actions != null)
            {
                TextManipulationActionContext textManipulationActionContext = new TextManipulationActionContext();
                TextManipulationTarget target = new TextManipulationTarget
                {
                    TextValue = cell != null ? cell.Value.ToString() : null
                };
                foreach (var action in this.ManipulationSettings.Actions)
                {
                    action.Execute(textManipulationActionContext, target);
                }
                return target.TextValue.Trim();
            }
            if (cell != null)
                return cell.Value;
            else
                return null;
        }
    }
}
