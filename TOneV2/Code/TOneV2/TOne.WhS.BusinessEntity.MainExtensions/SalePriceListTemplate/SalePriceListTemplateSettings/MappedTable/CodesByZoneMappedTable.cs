using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class CodesByZoneMappedTable : MappedTable
    {
        public override Guid ConfigId
        {
            get { return new Guid("0F8440CA-2B68-48C0-8E23-B4E0F7FAF719"); }
        }

        public char Delimiter { get; set; }

        public override IEnumerable<SalePriceListTemplateTableCell> FillSheet(ISalePriceListTemplateSettingsContext context, string dateTimeFormat)
        {
            List<SalePriceListTemplateTableCell> sheets = new List<SalePriceListTemplateTableCell>();
            int currentRowIndex = this.FirstRowIndex;

            foreach (SalePLZoneNotification zone in context.Zones)
            {
                IEnumerable<CodesByZoneMappedColumn> mappedCols = this.MappedColumns.Select(item => item as CodesByZoneMappedColumn);
                SetRecordData(sheets, mappedCols, zone, currentRowIndex++, dateTimeFormat);

            }
            return sheets;
        }

        private void SetRecordData(List<SalePriceListTemplateTableCell> sheets, IEnumerable<CodesByZoneMappedColumn> mappedCols, SalePLZoneNotification zone, int rowIndex, string dateTimeFormat)
        {
            foreach (CodesByZoneMappedColumn mappedCol in mappedCols)
                SetCellData(sheets, mappedCol, zone, rowIndex, dateTimeFormat);
        }


        private void SetCellData(List<SalePriceListTemplateTableCell> sheets, CodesByZoneMappedColumn mappedCol, SalePLZoneNotification zone, int rowIndex, string dateTimeFormat)
        {
            if (zone == null)
                throw new ArgumentNullException("zone");

            var mappedValueContext = new CodesByZoneMappedValueContext()
            {
                ZoneNotification = zone,
                Delimiter = this.Delimiter
            };


            mappedCol.MappedValue.Execute(mappedValueContext);

            if (mappedValueContext.Value != null && mappedValueContext.Value is DateTime)
                mappedValueContext.Value = ((DateTime)mappedValueContext.Value).ToString(dateTimeFormat);

            sheets.Add(new SalePriceListTemplateTableCell()
            {
                ColumnIndex = mappedCol.ColumnIndex,
                RowIndex = rowIndex,
                Value = mappedValueContext.Value
            });

        }
    }
}
