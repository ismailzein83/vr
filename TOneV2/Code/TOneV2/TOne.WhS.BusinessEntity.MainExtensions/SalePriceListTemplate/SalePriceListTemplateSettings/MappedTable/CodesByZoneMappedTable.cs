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

        public override IEnumerable<SalePriceListTemplateTableCell> FillSheet(ISalePriceListTemplateSettingsContext context, string dateTimeFormat)
        {
            List<SalePriceListTemplateTableCell> sheets = new List<SalePriceListTemplateTableCell>();
            int currentRowIndex = this.FirstRowIndex;

            foreach (SalePLZoneNotification zone in context.Zones)
            {
                IEnumerable<CodesByZoneMappedColumn> mappedCols = this.MappedColumns.Select(item => item as CodesByZoneMappedColumn);
                SetRecordData(sheets, mappedCols, zone, ref currentRowIndex, dateTimeFormat);
            }
            return sheets;
        }

        private void SetRecordData(List<SalePriceListTemplateTableCell> sheets, IEnumerable<CodesByZoneMappedColumn> mappedCols, SalePLZoneNotification zone, ref int rowIndex, string dateTimeFormat)
        {
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
                            Rate = zone.Rate,
                            Codes = codes
                        };
                        SetCellData(sheets, mappedCol, currentZone, rowIndex, dateTimeFormat);
                    }
                    rowIndex++;
                }
            }
            else
            {
                foreach (CodesByZoneMappedColumn mappedCol in mappedCols)
                    SetCellData(sheets, mappedCol, zone, rowIndex, dateTimeFormat);
                rowIndex++;
            }
               
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
