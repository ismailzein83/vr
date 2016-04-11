﻿using Aspose.Cells;
using ExcelConversion.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Business
{
    public class PriceListConversionManager
    {
        public byte[]  PriceListConvertAndDownload(PriceListConversion priceListConversion)
        {
            ExcelConvertor excelConvertor = new ExcelConvertor();
            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(priceListConversion.InputPriceListSettings.FileId, priceListConversion.InputPriceListSettings.ExcelConversionSettings);
            PriceListItem priceListItem = ConvertToPriceListItem(convertedExcel);
            Workbook wbk = new Workbook();
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
            wbk.Worksheets.Clear();
            int rowIndex = priceListConversion.OutputPriceListSettings.OutputPriceListFields.FirstRowIndex;
            int zoneCellIndex = priceListConversion.OutputPriceListSettings.OutputPriceListFields.ZoneCellIndex;
            int codeCellIndex =priceListConversion.OutputPriceListSettings.OutputPriceListFields.CodeCellIndex;
            int rateCellIndex = priceListConversion.OutputPriceListSettings.OutputPriceListFields.RateCellIndex;
            int effectiveDateCellIndex = priceListConversion.OutputPriceListSettings.OutputPriceListFields.EffectiveDateCellIndex;
            Worksheet RateWorkSheet = wbk.Worksheets.Add("Result");

            SetCellProperties(RateWorkSheet,  rowIndex,  zoneCellIndex, "Zone");
            SetCellProperties(RateWorkSheet,  rowIndex,  codeCellIndex, "Code");
            SetCellProperties(RateWorkSheet,  rowIndex,  rateCellIndex, "Rate");
            SetCellProperties(RateWorkSheet, rowIndex, effectiveDateCellIndex, "BED");
          
            
            rowIndex++;
            foreach (var item in priceListItem.Records)
            {
                RateWorkSheet.Cells[rowIndex, zoneCellIndex].PutValue(item.Zone);

                RateWorkSheet.Cells[rowIndex, codeCellIndex].PutValue(item.Code);

                RateWorkSheet.Cells[rowIndex, rateCellIndex].PutValue(item.Rate);

                RateWorkSheet.Cells[rowIndex, effectiveDateCellIndex].PutValue(item.BED);
                rowIndex++;
            }
 

            MemoryStream memoryStream = new MemoryStream();
            memoryStream = wbk.SaveToStream();

            return memoryStream.ToArray();
        }
        private void SetCellProperties(Worksheet workSheet, int rowIndex, int colIndex, string propName)
        {
            workSheet.Cells.SetColumnWidth(colIndex, 20);
            workSheet.Cells[rowIndex, colIndex].PutValue(propName);
            Cell cell = workSheet.Cells.GetCell(rowIndex, colIndex);
            Style style = cell.GetStyle();
            style.Font.Name = "Times New Roman";
            style.Font.Color = Color.FromArgb(255, 0, 0); ;
            style.Font.Size = 14;
            style.Font.IsBold = true;
            cell.SetStyle(style);
            colIndex++;
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
                        rateByZone.Add(zoneField.FieldValue.ToString(), Convert.ToDecimal(rateField.FieldValue));
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
                    if (obj.Fields.TryGetValue("BED", out bEDField))
                    {
                        priceListRecord.BED = (DateTime)bEDField.FieldValue;
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




        public byte[] ConvertAndDownload(ExcelToConvert excelToConvert)
        {
            ExcelConvertor excelConvertor = new ExcelConvertor();
            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(excelToConvert.FileId, excelToConvert.ExcelConversionSettings);
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
                        rateByZone.Add(zoneField.FieldValue.ToString(), Convert.ToDecimal(rateField.FieldValue));
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
                    if (obj.Fields.TryGetValue("BED", out bEDField))
                    {
                        priceListRecord.BED = (DateTime)bEDField.FieldValue;
                    };
                    decimal rate;
                    if (rateByZone.TryGetValue(priceListRecord.Zone, out rate))
                    {
                        priceListRecord.Rate = rate;
                    }
                    priceListItem.Records.Add(priceListRecord);
                }

            }


            //BinaryFormatter formatter = new BinaryFormatter();
            //MemoryStream memStream = new MemoryStream();
            //formatter.Serialize(memStream, excelTemplate.Records);


            Workbook wbk = new Workbook();
            Aspose.Cells.License license = new Aspose.Cells.License();
            license.SetLicense("Aspose.Cells.lic");
            wbk.Worksheets.Clear();
            Worksheet RateWorkSheet = wbk.Worksheets.Add("Result");
            int rowIndex = 0;
            int colIndex = 0;

            PropertyInfo[] properties = typeof(PriceListRecord).GetProperties();
            PropertyInfo entityProperty = null;
            PropertyInfo[] entityProperties = null;

            //filling header
            foreach (var prop in properties)
            {
                RateWorkSheet.Cells.SetColumnWidth(colIndex, 20);
                RateWorkSheet.Cells[rowIndex, colIndex].PutValue(prop.Name);
                Cell cell = RateWorkSheet.Cells.GetCell(rowIndex, colIndex);
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
                colIndex++;
            }
            if (entityProperties != null)
            {
                foreach (var prop in entityProperties)
                {
                    RateWorkSheet.Cells.SetColumnWidth(colIndex, 20);
                    RateWorkSheet.Cells[rowIndex, colIndex].PutValue(prop.Name);
                    Cell cell = RateWorkSheet.Cells.GetCell(rowIndex, colIndex);
                    Style style = cell.GetStyle();
                    style.Font.Name = "Times New Roman";
                    style.Font.Color = Color.FromArgb(255, 0, 0); ;
                    style.Font.Size = 14;
                    style.Font.IsBold = true;
                    cell.SetStyle(style);
                    colIndex++;
                }
            }
            rowIndex++;
            colIndex = 0;

            //filling result
            foreach (var item in priceListItem.Records)
            {
                colIndex = 0;
                foreach (var prop in properties)
                {
                    if (prop == entityProperty)
                        continue;
                    RateWorkSheet.Cells[rowIndex, colIndex].PutValue(prop.GetValue(item));
                    colIndex++;
                }
                if (entityProperty != null)
                {
                    object entityPropertyValue = entityProperty.GetValue(item);
                    if (entityPropertyValue != null)
                    {
                        foreach (var prop in entityProperties)
                        {
                            RateWorkSheet.Cells[rowIndex, colIndex].PutValue(prop.GetValue(entityPropertyValue));
                            colIndex++;
                        }
                    }
                }

                rowIndex++;
            }


            MemoryStream memoryStream = new MemoryStream();
            memoryStream = wbk.SaveToStream();

            return memoryStream.ToArray();

            //   return memStream.ToArray();
        }
    }
}
