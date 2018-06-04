using Aspose.Cells;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.ExcelConversion.Entities;

namespace Vanrise.ExcelConversion.Business
{
    public class ExcelManager
    {
        GeneralSettingsManager _generalSettingsManager = new GeneralSettingsManager();
        public ExcelWorkbook ReadExcelFile(long fileId)
        {
            var fileManager = new VRFileManager();
            var file = fileManager.GetFile(fileId);
            if (file == null)
                throw new NullReferenceException(String.Format("file '{0}'", fileId));
            if (file.Content == null)
                throw new NullReferenceException(String.Format("file.Content '{0}'", fileId));

            MemoryStream stream = new MemoryStream(file.Content);

            Common.Utilities.ActivateAspose();
            var workbook = CreateWorkbook(file, stream);
            ExcelWorkbook ewb = new ExcelWorkbook() { Sheets = new List<ExcelWorksheet>() };
            foreach (var sheet in workbook.Worksheets)
            {
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
                int maxDataRow = sheet.Cells.MaxDataRow;
                for (int j = 0; j < 100; j++)
                {
                    Row row = sheet.Cells.Rows[j] != null ? (Row)sheet.Cells.Rows[j] : null;
                    int maxdatacol = sheet.Cells.MaxDataColumn + 1;

                    ExcelRow eRow = new ExcelRow() { Cells = new List<ExcelCell>() };

                    int nbOfRowColumns = (row != null && row.LastCell != null) ? row.LastCell.Column + 1 : 0;
                    if (nbOfRowColumns > nbOfSheetColumns)
                        nbOfSheetColumns = nbOfRowColumns;
                    for (int colIndex = 0; colIndex < maxdatacol; colIndex++)
                    {
                        var cell = (row != null) ? row.GetCellOrNull(colIndex) : null;
                        ExcelCell eCell = new ExcelCell();
                        if (cell != null)
                        {
                            if (cell.Type == CellValueType.IsDateTime)
                            {
                                if (cell.Value != null)
                                {
                                    var valueDate = Convert.ToDateTime(cell.Value);
                                    if (valueDate != valueDate.Date)
                                    {
                                        eCell.Value = valueDate.ToString(_generalSettingsManager.GetLongDateTimeFormat());
                                    }
                                    else
                                    {
                                        eCell.Value = valueDate.ToString(_generalSettingsManager.GetDateFormat());
                                    }
                                }
                            }
                            else
                                eCell.Value = cell.Value;
                        }
                        else
                            eCell.Value = null;

                        eRow.Cells.Add(eCell);


                    }
                    // if (j < maxDataRow)
                    eSheet.Rows.Add(eRow);

                }
                eSheet.NumberOfColumns = nbOfSheetColumns;
                eSheet.MaxDataRow = maxDataRow;
            }

            return ewb;
        }
        public Workbook CreateWorkbook(VRFile file,MemoryStream stream)
        {
            if (file.Extension.ToLower().Equals("csv"))
            {
                TxtLoadOptions txtLoadOptions = new TxtLoadOptions();
                txtLoadOptions.Separator = Convert.ToChar(",");
                txtLoadOptions.Encoding = System.Text.Encoding.UTF8;
                return new Workbook(stream, txtLoadOptions);
            }
            else
               return new Workbook(stream);
        }

        public ExcelWorksheet ReadExcelFilePage(ExcelPageQuery Query)
        {

            var fileManager = new VRFileManager();
            var file = fileManager.GetFile(Query.FileId);
            if (file == null)
                throw new NullReferenceException(String.Format("file '{0}'", Query.FileId));
            if (file.Content == null)
                throw new NullReferenceException(String.Format("file.Content '{0}'", Query.FileId));
            MemoryStream stream = new MemoryStream(file.Content);
            Common.Utilities.ActivateAspose();
            var workbook = CreateWorkbook(file, stream);
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
            int maxDataRow = sheet.Cells.MaxDataRow;

            for (int j = Query.From; j < Query.To; j++)
            {
                Row row = sheet.Cells.Rows[j] != null ? (Row)sheet.Cells.Rows[j] : null;
                int maxdatacol = sheet.Cells.MaxDataColumn + 1;

                ExcelRow eRow = new ExcelRow() { Cells = new List<ExcelCell>() };
                int nbOfRowColumns = (row != null && row.LastCell != null) ? row.LastCell.Column + 1 : 0;
                if (nbOfRowColumns > nbOfSheetColumns)
                    nbOfSheetColumns = nbOfRowColumns;
                for (int colIndex = 0; colIndex < maxdatacol; colIndex++)
                {
                    var cell = (row != null) ? row.GetCellOrNull(colIndex) : null;
                    ExcelCell eCell = new ExcelCell();

                    if (cell != null)
                    {
                        if (cell.Type == CellValueType.IsDateTime)
                        {
                            if (cell.Value != null)
                            {
                                var valueDate = Convert.ToDateTime(cell.Value);
                                if (valueDate != valueDate.Date)
                                {
                                    eCell.Value = valueDate.ToString(_generalSettingsManager.GetLongDateTimeFormat());
                                }
                                else
                                {
                                    eCell.Value = valueDate.ToString(_generalSettingsManager.GetDateFormat());
                                }
                            }
                        }
                        else
                            eCell.Value = cell.Value;
                    }
                    else
                        eCell.Value = null;

                    eRow.Cells.Add(eCell);


                }
                eSheet.Rows.Add(eRow);


            }
            eSheet.NumberOfColumns = nbOfSheetColumns;
            return eSheet;

        }
        public IEnumerable<FieldMappingConfig> GetFieldMappingTemplateConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<FieldMappingConfig>(FieldMappingConfig.EXTENSION_TYPE).OrderByDescending(x => x.Name);
        }
        public IEnumerable<ConcatenatedPartConfig> GetConcatenatedPartTemplateConfigs()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<ConcatenatedPartConfig>(ConcatenatedPartConfig.EXTENSION_TYPE);
        }

        public IEnumerable<string> ReadConditionsFromFile(ReadConditionsFromFileInput input)
        {
            HashSet<string> conditions = new HashSet<string>();
            var excelConverted = new ExcelConvertor().ConvertExcelFile(input.FileId, input.ConversionSettings, true, false);
            if (excelConverted != null)
            {
                ConvertedExcelList convertedExcelList;
                if (excelConverted.Lists.TryGetValue("ConditionList", out convertedExcelList))
                {
                    if (convertedExcelList.Records != null)
                    {
                        foreach (var record in convertedExcelList.Records)
                        {
                            ConvertedExcelField excelField;
                            if (record.Fields.TryGetValue("Condition", out excelField))
                            {
                                if (excelField.FieldValue == null || String.IsNullOrWhiteSpace(excelField.FieldValue.ToString()))
                                    conditions.Add("");
                                else
                                    conditions.Add(excelField.FieldValue.ToString());
                            };
                            if (conditions.Count == 20)
                                break;
                        }
                    }
                }
            }
            return conditions;
        }
    }
}
