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

        public override IEnumerable<SalePricelistTemplateTableRow> BuildSheet(IEnumerable<SalePLZoneNotification> zoneNotificationList, string dateTimeFormat, int customerId)
        {
            List<SalePricelistTemplateTableRow> sheet = new List<SalePricelistTemplateTableRow>();
            int currentRowIndex = this.FirstRowIndex;

            foreach (SalePLZoneNotification zone in zoneNotificationList)
            {
                foreach (var code in zone.Codes)
                {
                    IEnumerable<CodeOnEachRowMappedColumn> concreteMappedColumns = this.MappedColumns.Select(item => item as CodeOnEachRowMappedColumn);
                    sheet.Add(GetRowData(concreteMappedColumns, zone, code, zone.Rate, currentRowIndex++, dateTimeFormat, customerId));
                }

            }
            return sheet;
        }

        private SalePricelistTemplateTableRow GetRowData(IEnumerable<CodeOnEachRowMappedColumn> mappedCols, SalePLZoneNotification zone, SalePLCodeNotification code, SalePLRateNotification rate, int rowIndex, string dateTimeFormat, int customerId)
        {
            var row = new SalePricelistTemplateTableRow
            {
                RowCells = new List<SalePriceListTemplateTableCell>(),
            };

            foreach (CodeOnEachRowMappedColumn mappedCol in mappedCols)
            {
                row.RowCells.Add(GetCellData(mappedCol, zone, code, rate, rowIndex, dateTimeFormat, customerId));
            }
            return row;
        }


        private SalePriceListTemplateTableCell GetCellData(CodeOnEachRowMappedColumn mappedCol, SalePLZoneNotification zone, SalePLCodeNotification code, SalePLRateNotification rate, int rowIndex, string dateTimeFormat, int customerId)
        {
            if (zone == null)
                throw new ArgumentNullException("zone");

            var mappedValueContext = new CodeOnEachRowMappedValueContext
            {
                Zone = zone.ZoneName,
                CustomerId = customerId,
                OtherRateByRateTypeId = zone.OtherRateByRateTypeId
            };

            if (code != null)
            {
                mappedValueContext.Code = code.Code;
                mappedValueContext.CodeBED = code.BED;
                mappedValueContext.CodeEED = code.EED;
                mappedValueContext.CodeChangeType = code.CodeChange;
            }

            if (rate != null)
            {
                mappedValueContext.Rate = rate.Rate;
                mappedValueContext.RateBED = rate.BED;
                mappedValueContext.RateEED = rate.EED;
                mappedValueContext.ServicesIds = rate.ServicesIds;
                mappedValueContext.RateChangeType = rate.RateChangeType;
                mappedValueContext.CurrencyId = rate.CurrencyId;
                mappedValueContext.Increment = zone.Increment;
            }

            mappedCol.MappedValue.Execute(mappedValueContext);

            if (mappedValueContext.Value is DateTime)
                mappedValueContext.Value = ((DateTime)mappedValueContext.Value).ToString(dateTimeFormat);

            return (new SalePriceListTemplateTableCell
            {
                ColumnIndex = mappedCol.ColumnIndex,
                Value = mappedValueContext.Value
            });
        }
    }
}
