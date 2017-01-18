using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class CodeOnEachRowMappedTable : MappedTable 
    {
        public override Guid ConfigId
        {
            get { return new Guid("4EBE3013-2A48-41BD-97D6-57286759B907"); }
        }

        public override IEnumerable<SalePriceListTemplateTableCell> FillSheet(ISalePriceListTemplateSettingsContext context, string dateTimeFormat)
        {
            List<SalePriceListTemplateTableCell> sheets = new List<SalePriceListTemplateTableCell>();
            int currentRowIndex = this.FirstRowIndex;

            foreach (SalePLZoneNotification zone in context.Zones)
            {
                foreach (var code in zone.Codes)
                    {
                        IEnumerable<CodeOnEachRowMappedColumn> concreteMappedColumns = this.MappedColumns.Select(item => item as CodeOnEachRowMappedColumn);
                        SetRecordData(sheets, concreteMappedColumns, zone, code, zone.Rate, currentRowIndex++, dateTimeFormat);
                    }
                        
            }
            return sheets;
        }

        private void SetRecordData(List<SalePriceListTemplateTableCell> sheets, IEnumerable<CodeOnEachRowMappedColumn> mappedCols, SalePLZoneNotification zone, SalePLCodeNotification code, SalePLRateNotification rate, int rowIndex, string dateTimeFormat)
        {
            foreach (CodeOnEachRowMappedColumn mappedCol in mappedCols)
                SetCellData(sheets, mappedCol, zone, code, rate, rowIndex, dateTimeFormat);
        }


        private void SetCellData(List<SalePriceListTemplateTableCell> sheets, CodeOnEachRowMappedColumn mappedCol, SalePLZoneNotification zone, SalePLCodeNotification code, SalePLRateNotification rate, int rowIndex, string dateTimeFormat)
        {
            if (zone == null)
                throw new ArgumentNullException("zone");

            var mappedValueContext = new CodeOnEachRowMappedValueContext()
            {
                Zone = zone.ZoneName
            };

            if (code != null)
            {
                mappedValueContext.Code = code.Code;
                mappedValueContext.CodeBED = code.BED;
                mappedValueContext.CodeEED = code.EED;
            }

            if (rate != null)
            {
                mappedValueContext.Rate = rate.Rate;
                mappedValueContext.RateBED = rate.BED;
                mappedValueContext.RateEED = rate.EED;
            }

            mappedCol.MappedValue.Execute(mappedValueContext);

            if (mappedValueContext.Value != null)
            {
                if (mappedValueContext.Value is DateTime)
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
}
