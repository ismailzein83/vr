using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.ExcelConversion.Business;
using Vanrise.ExcelConversion.Entities;
using XBooster.PriceListConversion.Entities;

namespace XBooster.PriceListConversion.Business
{
    public class PriceListConversionManager
    {
        public byte[] PriceListConvertAndDownload(Entities.PriceListConversion priceListConversion)
        {
            ExcelConvertor excelConvertor = new ExcelConvertor();
            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(priceListConversion.InputPriceListSettings.FileId, priceListConversion.InputPriceListSettings.ExcelConversionSettings);
            PriceListItem priceListItem = ConvertToPriceListItem(convertedExcel);


            var fileManager = new VRFileManager();
            var file = fileManager.GetFile(priceListConversion.OutputPriceListSettings.FileId);
            if (file == null)
                throw new NullReferenceException(String.Format("file '{0}'", priceListConversion.OutputPriceListSettings.FileId));
            if (file.Content == null)
                throw new NullReferenceException(String.Format("file.Content '{0}'", priceListConversion.OutputPriceListSettings.FileId));
            MemoryStream stream = new MemoryStream(file.Content);
            Workbook workbook = new Workbook(stream);
            var workSheet = workbook.Worksheets[priceListConversion.OutputPriceListSettings.OutputPriceListFields.SheetIndex];
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
            int rowIndex = priceListConversion.OutputPriceListSettings.OutputPriceListFields.FirstRowIndex;
            int zoneCellIndex = priceListConversion.OutputPriceListSettings.OutputPriceListFields.ZoneCellIndex;
            int codeCellIndex = priceListConversion.OutputPriceListSettings.OutputPriceListFields.CodeCellIndex;
            int rateCellIndex = priceListConversion.OutputPriceListSettings.OutputPriceListFields.RateCellIndex;
            int effectiveDateCellIndex = priceListConversion.OutputPriceListSettings.OutputPriceListFields.EffectiveDateCellIndex;


            foreach (var item in priceListItem.Records)
            {
                workSheet.Cells[rowIndex, zoneCellIndex].PutValue(item.Zone);

                workSheet.Cells[rowIndex, codeCellIndex].PutValue(item.Code);

                workSheet.Cells[rowIndex, rateCellIndex].PutValue(item.Rate);

                workSheet.Cells[rowIndex, effectiveDateCellIndex].PutValue(item.EffectiveDate);
                rowIndex++;
            }


            MemoryStream memoryStream = new MemoryStream();
            memoryStream = workbook.SaveToStream();

            return memoryStream.ToArray();
        }

        private PriceListItem ConvertToPriceListItem(ConvertedExcel convertedExcel)
        {
            PriceListItem priceListItem = new Entities.PriceListItem();
            priceListItem.Records = new List<PriceListRecord>();
            Dictionary<string, decimal> rateByZone = new Dictionary<string, decimal>();
            ConvertedExcelList RateConvertedExcelList;
            if (convertedExcel.Lists.TryGetValue("RateList", out RateConvertedExcelList))
            {
                foreach (var obj in RateConvertedExcelList.Records)
                {
                    ConvertedExcelField zoneField;
                    ConvertedExcelField rateField;
                    if (obj.Fields.TryGetValue("Zone", out zoneField) && obj.Fields.TryGetValue("Rate", out rateField))
                    {
                        decimal rate;
                        if (!rateByZone.TryGetValue(zoneField.FieldValue.ToString(), out rate))
                        {
                            rateByZone.Add(zoneField.FieldValue.ToString(), Convert.ToDecimal(rateField.FieldValue));
                        }
                        else
                        {
                            if (rate == Convert.ToDecimal(rateField.FieldValue))
                            {
                                continue;

                            }
                            else
                            {
                                throw new Exception(string.Format("Same zone {0} of different rate exists.", zoneField.FieldValue));
                            }
                        }

                    };
                }

            }

            ConvertedExcelList CodeConvertedExcelList;
            if (convertedExcel.Lists.TryGetValue("CodeList", out CodeConvertedExcelList))
            {
                foreach (var obj in CodeConvertedExcelList.Records)
                {
                    PriceListRecord priceListRecord = new Entities.PriceListRecord();
                    ConvertedExcelField zoneField;

                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {
                        priceListRecord.Zone = zoneField.FieldValue.ToString();
                    };
                    ConvertedExcelField codeField;
                    if (obj.Fields.TryGetValue("Code", out codeField))
                    {
                        priceListRecord.Code = codeField.FieldValue.ToString();
                    };
                    ConvertedExcelField bEDField;
                    if (obj.Fields.TryGetValue("EffectiveDate", out bEDField))
                    {
                        priceListRecord.EffectiveDate = (DateTime)bEDField.FieldValue;
                    };
                    decimal rate;
                    if (rateByZone.TryGetValue(priceListRecord.Zone, out rate))
                    {
                        priceListRecord.Rate = rate;
                    }
                    priceListItem.Records.Add(priceListRecord);
                }

            }
            return priceListItem;
        }

    }
}
