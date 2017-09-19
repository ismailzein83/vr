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
        private const int _excelCellMaxValue = 32767;
        public override Guid ConfigId
        {
            get { return new Guid("0F8440CA-2B68-48C0-8E23-B4E0F7FAF719"); }
        }

        public char Delimiter { get; set; }

        public override IEnumerable<SalePricelistTemplateTableRow> BuildSheet(IEnumerable<SalePLZoneNotification> zoneNotificationList, string dateTimeFormat)
        {
            List<SalePricelistTemplateTableRow> sheet = new List<SalePricelistTemplateTableRow>();
            int currentRowIndex = this.FirstRowIndex;

            foreach (SalePLZoneNotification zone in zoneNotificationList)
            {
                IEnumerable<CodesByZoneMappedColumn> mappedCols = this.MappedColumns.Select(item => item as CodesByZoneMappedColumn);
                sheet.Add(GetRowData(mappedCols, zone, currentRowIndex++, dateTimeFormat));
            }
            return sheet;
        }

        private SalePricelistTemplateTableRow GetRowData(IEnumerable<CodesByZoneMappedColumn> mappedCols, SalePLZoneNotification zone, int rowIndex, string dateTimeFormat)
        {
            var row = new SalePricelistTemplateTableRow
            {
                RowCells = new List<SalePriceListTemplateTableCell>(),
            };

            //if (zone.Codes.All(x => x.EED.HasValue))
            //    return;//No need to mention the zone if all codes are closed for it (this happens when renaming a zone also)

            int? codesLength = GetLength(zone.Codes.Select(itm => itm.Code));

            int splitedCodesCount = 0;

            if (codesLength.HasValue && codesLength > _excelCellMaxValue)
                splitedCodesCount = (int)Math.Ceiling((double)codesLength.Value / _excelCellMaxValue);

            if (splitedCodesCount > 0)
            {
                int saleCodesStartIndex = 0;
                for (int i = 0; i < splitedCodesCount; i++)
                {
                    List<SalePLCodeNotification> codes = GetSaleCodes(zone.Codes, ref saleCodesStartIndex);
                    foreach (CodesByZoneMappedColumn mappedCol in mappedCols)
                    {
                        SalePLZoneNotification currentZone = new SalePLZoneNotification()
                        {
                            ZoneName = zone.ZoneName,
                            ZoneId = zone.ZoneId,
                            Rate = zone.Rate
                        };
                        currentZone.Codes.AddRange(codes);
                        row.RowCells.Add(GetCellData( mappedCol, currentZone, rowIndex, dateTimeFormat));
                    }
                }
            }
            else
            {
                foreach (CodesByZoneMappedColumn mappedCol in mappedCols)
                    row.RowCells.Add(GetCellData( mappedCol, zone, rowIndex, dateTimeFormat));
            }
            return row;

        }

        private SalePriceListTemplateTableCell GetCellData( CodesByZoneMappedColumn mappedCol, SalePLZoneNotification zone, int rowIndex, string dateTimeFormat)
        {
            if (zone == null)
                throw new ArgumentNullException("zone");

            var mappedValueContext = new CodesByZoneMappedValueContext()
            {
                ZoneNotification = zone,
                Delimiter = this.Delimiter
            };

            mappedCol.MappedValue.Execute(mappedValueContext);

                if (mappedValueContext.Value is DateTime)
                    mappedValueContext.Value = ((DateTime)mappedValueContext.Value).ToString(dateTimeFormat);

                return(new SalePriceListTemplateTableCell()
                {
                    ColumnIndex = mappedCol.ColumnIndex,
                    Value = mappedValueContext.Value
                });

        }

        private List<SalePLCodeNotification> GetSaleCodes(List<SalePLCodeNotification> codes, ref int startIndex)
        {
            List<SalePLCodeNotification> saleCodes = new List<SalePLCodeNotification>();
            if (codes != null)
            {
                List<SalePLCodeNotification> salePLCodesNotification = codes.GetRange(startIndex, codes.Count() - startIndex);
                foreach (SalePLCodeNotification code in salePLCodesNotification)
                {
                    int? codesLenth = GetLength(saleCodes.Select(item => item.Code));
                    if (codesLenth.HasValue && (codesLenth.Value + code.Code.Length < _excelCellMaxValue))
                    {
                        saleCodes.Add(code);
                        startIndex++;
                    }
                    else
                        break;
                }
            }
            return saleCodes;
        }

        private int? GetLength(IEnumerable<string> codes)
        {
            if (codes == null)
                return null;

            int codesCount = 0;
            foreach (string code in codes)
            {
                codesCount += code.Length;
            }
            return codesCount + codes.Count() - 1;
        }
    }
}
