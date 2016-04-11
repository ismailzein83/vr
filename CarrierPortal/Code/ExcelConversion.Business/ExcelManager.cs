using Aspose.Cells;
using ExcelConversion.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace ExcelConversion.Business
{
    public class ExcelManager
    {
        public ExcelWorkbook ReadExcelFile(long fileId)
        {
            var fileManager = new VRFileManager();
            var file = fileManager.GetFile(fileId);
            if (file == null)
                throw new NullReferenceException(String.Format("file '{0}'", fileId));
            if (file.Content == null)
                throw new NullReferenceException(String.Format("file.Content '{0}'", fileId));
            MemoryStream stream = new MemoryStream(file.Content);
            Workbook workbook = new Workbook(stream);

            ExcelWorkbook ewb = new ExcelWorkbook() { Sheets = new List<ExcelWorksheet>() };
            foreach(var sheet in workbook.Worksheets)
            {
                

                ExcelWorksheet eSheet = new ExcelWorksheet() { 
                    Rows = new List<ExcelRow>() ,
                    MergedCells = new List<MergedCell>()
                };
                ArrayList mcells = sheet.Cells.MergedCells;



                for (int i = 0; i < mcells.Count; i++)
                {

                    CellArea area = (CellArea)mcells[i];
                    MergedCell mc = new MergedCell();
                    mc.col = area.StartColumn ;
                    mc.row = area.StartRow ;
                    mc.rowspan = (area.EndRow - area.StartRow) +1;
                    mc.colspan = (area.EndColumn - area.StartColumn)+1 ;
                    eSheet.MergedCells.Add(mc);

                }

                ewb.Sheets.Add(eSheet);
                eSheet.Name = sheet.Name;
                int nbOfSheetColumns = 0;
                for (int j = 0; j <= 100; j++)
                {
                    Row row = (Row)sheet.Cells.Rows[j];

                    if (row != null && !row.IsBlank)
                    {
                        ExcelRow eRow = new ExcelRow() { Cells = new List<ExcelCell>() };
                        eSheet.Rows.Add(eRow);
                        int nbOfRowColumns = row.LastCell.Column + 1;
                        if (nbOfRowColumns > nbOfSheetColumns)
                            nbOfSheetColumns = nbOfRowColumns;
                        for (int colIndex = 0; colIndex < nbOfRowColumns; colIndex++)
                        {
                            var cell = row.GetCellOrNull(colIndex);
                            ExcelCell eCell = new ExcelCell();
                            if (cell != null)
                            {
                                eCell.Value = cell.Value;
                                eRow.Cells.Add(eCell);

                            }

                        }
                    }


                }
                eSheet.NumberOfColumns = nbOfSheetColumns;
            }

            return ewb;
        }

        public ExcelWorksheet ReadExcelFilePage(ExcelPageQuery Query)
        {
            System.Threading.Thread.Sleep(2000);

            var fileManager = new VRFileManager();
            var file = fileManager.GetFile(Query.FileId);
            if (file == null)
                throw new NullReferenceException(String.Format("file '{0}'", Query.FileId));
            if (file.Content == null)
                throw new NullReferenceException(String.Format("file.Content '{0}'", Query.FileId));
            MemoryStream stream = new MemoryStream(file.Content);
            Workbook workbook = new Workbook(stream);

            ExcelWorkbook ewb = new ExcelWorkbook() { Sheets = new List<ExcelWorksheet>() };
            var sheet = workbook.Worksheets[Query.SheetIndex];
            if (sheet == null)
                throw new NullReferenceException(String.Format("Sheet.Content '{0}'", Query.SheetIndex));
            


                ExcelWorksheet eSheet = new ExcelWorksheet()
                {
                    Rows = new List<ExcelRow>(),
                    MergedCells = new List<MergedCell>()
                };
                ArrayList mcells = sheet.Cells.MergedCells;



                for (int i = 0; i < mcells.Count; i++)
                {

                    CellArea area = (CellArea)mcells[i];
                    MergedCell mc = new MergedCell();
                    mc.col = area.StartColumn;
                    mc.row = area.StartRow;
                    mc.rowspan = (area.EndRow - area.StartRow) + 1;
                    mc.colspan = (area.EndColumn - area.StartColumn) + 1;
                    eSheet.MergedCells.Add(mc);

                }

                ewb.Sheets.Add(eSheet);
                eSheet.Name = sheet.Name;
                int nbOfSheetColumns = 0;

                for (int j = Query.From ; j < Query.To; j++)
                {
                    Row row = (Row)sheet.Cells.Rows[j];

                    if (row != null && !row.IsBlank)
                    {
                        ExcelRow eRow = new ExcelRow() { Cells = new List<ExcelCell>() };
                        eSheet.Rows.Add(eRow);
                        int nbOfRowColumns = row.LastCell.Column + 1;
                        if (nbOfRowColumns > nbOfSheetColumns)
                            nbOfSheetColumns = nbOfRowColumns;
                        for (int colIndex = 0; colIndex < nbOfRowColumns; colIndex++)
                        {
                            var cell = row.GetCellOrNull(colIndex);
                            ExcelCell eCell = new ExcelCell();
                            if (cell != null)
                            {
                                eCell.Value = cell.Value;
                                eRow.Cells.Add(eCell);

                            }

                        }
                    }
            
                    
                }
                eSheet.NumberOfColumns = nbOfSheetColumns;
               return eSheet;
           
        }
        public IEnumerable<TemplateConfig> GetFieldMappingTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.FieldMappingConfigType);
        }

        public byte[] ConvertAndDownload(ExcelToConvert excelToConvert)
        {
            ExcelConvertor excelConvertor = new ExcelConvertor();
            ConvertedExcel convertedExcel = excelConvertor.ConvertExcelFile(excelToConvert.FileId, excelToConvert.ExcelConversionSettings);
            ExcelTemplate excelTemplate = new Entities.ExcelTemplate();
            excelTemplate.Records = new List<ExcelTemplateRecord>();
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
                        rateByZone.Add(zoneField.FieldValue.ToString(),Convert.ToDecimal(rateField.FieldValue));
                    };
                }

            }

            ConvertedExcelList CodeConvertedExcelList;
            if (convertedExcel.Lists.TryGetValue("CodeList",out CodeConvertedExcelList))
            {
                foreach (var obj in CodeConvertedExcelList.Records)
                {
                    ExcelTemplateRecord excelTemplateRecord = new Entities.ExcelTemplateRecord();
                    ConvertedExcelField zoneField;

                    if (obj.Fields.TryGetValue("Zone", out zoneField))
                    {
                        excelTemplateRecord.Zone = zoneField.FieldValue.ToString();
                    };
                    ConvertedExcelField codeField;
                    if (obj.Fields.TryGetValue("Code", out codeField))
                    {
                        excelTemplateRecord.Code = codeField.FieldValue.ToString();
                    };
                    ConvertedExcelField bEDField;
                    if (obj.Fields.TryGetValue("BED", out bEDField))
                    {
                        excelTemplateRecord.BED = (DateTime)bEDField.FieldValue;
                    };
                    decimal rate;
                    if (rateByZone.TryGetValue(excelTemplateRecord.Zone,out rate))
                    {
                        excelTemplateRecord.Rate = rate;
                    }
                    excelTemplate.Records.Add(excelTemplateRecord);
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

            PropertyInfo[] properties = typeof(ExcelTemplateRecord).GetProperties();
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
            foreach (var item in excelTemplate.Records)
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
