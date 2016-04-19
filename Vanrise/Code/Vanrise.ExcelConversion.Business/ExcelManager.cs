﻿using Aspose.Cells;
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
                for (int j = 0; j <= 100; j++)
                {
                    Row row = sheet.Cells.Rows[j]!=null ?(Row)sheet.Cells.Rows[j] :null;
                    int maxDataRow = sheet.Cells.MaxDataRow;
                    int maxdatacol = sheet.Cells.MaxDataColumn;
                    ExcelRow eRow = new ExcelRow() { Cells = new List<ExcelCell>() };
                    eSheet.Rows.Add(eRow);
                    if ( j<= maxDataRow)
                    {
                        int nbOfRowColumns = (row != null && row.LastCell!=null) ? row.LastCell.Column + 1 : 0;
                        if (nbOfRowColumns > nbOfSheetColumns)
                            nbOfSheetColumns = nbOfRowColumns;
                        for (int colIndex = 0; colIndex < maxdatacol; colIndex++)
                        {
                            var cell = (row!=null)?row.GetCellOrNull(colIndex):null;
                            ExcelCell eCell = new ExcelCell();
                           
                            eCell.Value = (cell != null)?cell.Value:null;
                            eRow.Cells.Add(eCell);


                        }
                    }


                }
                eSheet.NumberOfColumns = nbOfSheetColumns;
            }

            return ewb;
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

            for (int j = Query.From; j <= Query.To; j++)
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
        public IEnumerable<TemplateConfig> GetConcatenatedPartTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.ConcatenatedPartConfigType);
        }
    }
}
