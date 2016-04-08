using Aspose.Cells;
using ExcelConversion.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace ExcelConversion.Business
{
    public class ExcelConvertor
    {
        #region Public Methods

        public ConvertedExcel ConvertExcelFile(long fileId, ExcelConversionSettings conversionSettings)
        {
            ConvertedExcel convertedExcel = new ConvertedExcel
            {
                Fields = new ConvertedExcelFieldsByName(),
                Lists = new ConvertedExcelListsByName()
            };

            var fileManager = new VRFileManager();
            var file = fileManager.GetFile(fileId);
            if (file == null)
                throw new NullReferenceException(String.Format("file '{0}'", fileId));
            if (file.Content == null)
                throw new NullReferenceException(String.Format("file.Content '{0}'", fileId));
            MemoryStream stream = new MemoryStream(file.Content);
            Workbook workbook = new Workbook(stream);

            ConvertFields(conversionSettings, convertedExcel, workbook);
            ConvertLists(conversionSettings, convertedExcel, workbook);

            return convertedExcel;
        }

        #endregion

        #region Private Methods

        private void ConvertFields(ExcelConversionSettings conversionSettings, ConvertedExcel convertedExcel, Workbook workbook)
        {
            if (conversionSettings.FieldMappings != null)
            {
                foreach (var fldMapping in conversionSettings.FieldMappings)
                {
                    ConvertedExcelField fld = new ConvertedExcelField
                    {
                        FieldName = fldMapping.FieldName,
                        FieldValue = GetFieldValue(workbook, fldMapping, conversionSettings, null, null)
                    };
                    convertedExcel.Fields.Add(fld);
                }
            }
        }

        private void ConvertLists(ExcelConversionSettings conversionSettings, ConvertedExcel convertedExcel, Workbook workbook)
        {
            if (conversionSettings.ListMappings != null)
            {
                foreach (var listMapping in conversionSettings.ListMappings)
                {
                    ConvertedExcelList lst = new ConvertedExcelList
                    {
                        ListName = listMapping.ListName,
                        Records = new List<ConvertedExcelRecord>()
                    };
                    if (workbook.Worksheets.Count <= listMapping.SheetIndex)
                        throw new Exception(String.Format("List SheetIndex '{0}' is greater than max index in the workbook", listMapping.SheetIndex));
                    var workSheet = workbook.Worksheets[listMapping.SheetIndex];
                    int lastRowIndex = listMapping.LastRowIndex.HasValue && listMapping.LastRowIndex.Value < workSheet.Cells.Rows.Count ? listMapping.LastRowIndex.Value : (workSheet.Cells.Rows.Count - 1);
                    for (int i = listMapping.FirstRowIndex; i <= lastRowIndex; i++)
                    {
                        var row = workSheet.Cells.Rows[i];
                        var convertedRecord = new ConvertedExcelRecord { Fields = new ConvertedExcelFieldsByName() };
                        lst.Records.Add(convertedRecord);

                        foreach (var fldMapping in listMapping.FieldMappings)
                        {
                            ConvertedExcelField fld = new ConvertedExcelField
                            {
                                FieldName = fldMapping.FieldName,
                                FieldValue = GetFieldValue(workbook, fldMapping, conversionSettings, workSheet, row)
                            };
                            convertedRecord.Fields.Add(fld);
                        }
                    }
                    convertedExcel.Lists.Add(lst);
                }
            }
        }

        private object GetFieldValue(Workbook workbook, FieldMapping fldMapping, ExcelConversionSettings conversionSettings, Worksheet workSheet, Row row)
        {
            GetFieldValueContext getFieldValueContext = new GetFieldValueContext
                {
                    Workbook = workbook,
                    Sheet = workSheet,
                    Row = row
                };
            Object fldValue = fldMapping.GetFieldValue(getFieldValueContext);
            if (fldValue == null)
                return null;
            if (fldMapping.FieldType == FieldType.DateTime)
            {
                if (fldValue is DateTime)
                    return fldValue;
                else
                    return DateTime.ParseExact(fldValue.ToString(), conversionSettings.DateTimeFormat, CultureInfo.CurrentCulture);
            }
            else
                return fldValue;
        }

        #endregion
    }
}
