using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.ExcelConversion.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.ExcelConversion.Business
{
    public class ExcelConvertor
    {
        #region Public Methods

        public ConvertedExcel ConvertExcelFile(long fileId, ExcelConversionSettings conversionSettings, bool stopOnFirstEmptyRow, bool isCommaDecimalSeparator)
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
            Common.Utilities.ActivateAspose();
            ConvertFields(conversionSettings, convertedExcel, workbook,isCommaDecimalSeparator);
            ConvertLists(conversionSettings, convertedExcel, workbook, stopOnFirstEmptyRow,isCommaDecimalSeparator);

            return convertedExcel;
        }

        #endregion

        #region Private Methods

        private void ConvertFields(ExcelConversionSettings conversionSettings, ConvertedExcel convertedExcel, Workbook workbook, bool isCommaDecimalSeparator)
        {
            if (conversionSettings.FieldMappings != null)
            {
                foreach (var fldMapping in conversionSettings.FieldMappings)
                {
                    ConvertedExcelField fld = new ConvertedExcelField
                    {
                        FieldName = fldMapping.FieldName,
                        FieldValue = GetFieldValue(workbook, fldMapping, conversionSettings, null, null,isCommaDecimalSeparator)
                    };
                    convertedExcel.Fields.Add(fld);
                }
            }
        }

        private void ConvertLists(ExcelConversionSettings conversionSettings, ConvertedExcel convertedExcel, Workbook workbook, bool stopOnFirstEmptyRow, bool isCommaDecimalSeparator)
        {
            if (conversionSettings.ListMappings != null)
            {
                foreach (var listMapping in conversionSettings.ListMappings)
                {

                    if (workbook.Worksheets.Count <= listMapping.SheetIndex)
                        throw new Exception(String.Format("List SheetIndex '{0}' is greater than max index in the workbook", listMapping.SheetIndex));
                    var workSheet = workbook.Worksheets[listMapping.SheetIndex];
                    int lastRowIndex = listMapping.LastRowIndex.HasValue && listMapping.LastRowIndex.Value <= workSheet.Cells.MaxDataRow ? listMapping.LastRowIndex.Value : (workSheet.Cells.MaxDataRow);

                    Dictionary<string, DataRecordFieldType> fieldTypeByFieldName = null;
                    if (listMapping.Filter != null && listMapping.Filter.Fields != null && listMapping.Filter.Fields.Count >0)
                    {
                        fieldTypeByFieldName = new Dictionary<string, DataRecordFieldType>();
                        foreach(var field in listMapping.Filter.Fields)
                        {
                            DataRecordFieldType fieldType = null;
                            if (!fieldTypeByFieldName.TryGetValue(field.FieldName, out fieldType))
                                fieldTypeByFieldName.Add(field.FieldName, field.FieldType);
                        }
                    }


                    BuildExceRecord(conversionSettings, convertedExcel, listMapping, workbook, workSheet, lastRowIndex, stopOnFirstEmptyRow, isCommaDecimalSeparator, fieldTypeByFieldName);
                }
            }
        }

        private void BuildExceRecord(ExcelConversionSettings conversionSettings, ConvertedExcel convertedExcel, ListMapping listMapping, Workbook workbook, Worksheet workSheet, int lastRowIndex, bool stopOnFirstEmptyRow, bool isCommaDecimalSeparator, Dictionary<string, DataRecordFieldType> fieldTypeByFieldName)
        {
            ConvertedExcelList lst = new ConvertedExcelList
            {
                ListName = listMapping.ListName,
                Records = new List<ConvertedExcelRecord>()
            };

            int maxdatacol = workSheet.Cells.MaxDataColumn + 1;
            for (int i = listMapping.FirstRowIndex; i <= lastRowIndex; i++)
            {
                var row = workSheet.Cells.Rows[i];

                if (stopOnFirstEmptyRow && CheckIsRowEmpty(row, maxdatacol))
                {
                    break;
                }

                if (fieldTypeByFieldName != null)
                {
                    RecordFilterManager manager = new RecordFilterManager();
                    MappingFilterMatchContext context = new MappingFilterMatchContext();
                    context.fieldTypeByFieldName = fieldTypeByFieldName;
                    Dictionary<string, Object> fieldValueByFieldName = new Dictionary<string, object>() ;
                    foreach(var field in listMapping.Filter.Fields)
                    {
                        object fieldValue = null;
                        if(!fieldValueByFieldName.TryGetValue(field.FieldName,out fieldValue))
                        {
                            fieldValueByFieldName.Add(field.FieldName, GetFieldValue(workbook, field.FieldMapping, conversionSettings, workSheet, row, isCommaDecimalSeparator));
                        }
                    }
                    context.fieldValueByFieldName = fieldValueByFieldName;
                    if (!manager.IsFilterGroupMatch(listMapping.Filter.FilterGroup, context))
                        continue;
                }

                var convertedRecord = new ConvertedExcelRecord { Fields = new ConvertedExcelFieldsByName() };

                lst.Records.Add(convertedRecord);
                foreach (var fldMapping in listMapping.FieldMappings)
                {
                    ConvertedExcelField fld = new ConvertedExcelField
                    {
                        FieldName = fldMapping.FieldName,
                        FieldValue = GetFieldValue(workbook, fldMapping, conversionSettings, workSheet, row,isCommaDecimalSeparator)
                    };
                    convertedRecord.Fields.Add(fld);
                }


            }
            convertedExcel.Lists.Add(lst);
        }

        private object GetFieldValue(Workbook workbook, FieldMapping fldMapping, ExcelConversionSettings conversionSettings, Worksheet workSheet, Row row, bool isCommaDecimalSeparator)
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
                {
                    DateTime result;
                    if (DateTime.TryParseExact(fldValue.ToString(), conversionSettings.DateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out result))
                    {
                        return result;
                    }
                    else
                    {
                        throw new Exception(string.Format("{0} is mapped to an invalid field.", fldMapping.FieldName));
                    }
                }
            }
            else if(fldMapping.FieldType == FieldType.Decimal)
            {
                if (fldValue is Decimal)
                    return fldValue;
                else
                {
                    Decimal result;
                    if (isCommaDecimalSeparator && Decimal.TryParse(fldValue.ToString().Replace(",", "."), out result))
                    {
                        return result;
                    }
                    else if (!isCommaDecimalSeparator && (Decimal.TryParse(fldValue.ToString(),NumberStyles.Any,CultureInfo.InvariantCulture, out result)))
                    {
                        return result;
                    }else
                    {
                        throw new Exception(string.Format("{0} is mapped to an invalid field.",fldMapping.FieldName));
                    }
                }
            }
            else
                return fldValue;
        }

        private bool CheckIsRowEmpty(Row row, int maxdatacol)
        {
            for(var i=0; i<=maxdatacol; i++)
            {
                var cell = row.GetCellOrNull(i);
                if(cell !=null && cell.Value !=null)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
