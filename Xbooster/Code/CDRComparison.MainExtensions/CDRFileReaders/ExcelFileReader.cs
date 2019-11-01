﻿using Aspose.Cells;
using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.MainExtensions
{
    public class ExcelFileReader : CDRFileReader
    {
        public override Guid ConfigId { get { return new Guid("61e3f8d9-5e5b-49ef-9ae3-df77bf1d674f"); } }

        #region Properties

        public int FirstRowIndex { get; set; }

        public List<ExcelFileFieldMapping> FieldMappings { get; set; }

        public string DateTimeFormat { get; set; }

        #endregion

        #region Override

        public override void ReadCDRs(IReadCDRsFromFileContext context)
        {
            if (this.FieldMappings == null)
                throw new ArgumentNullException("FieldMappings");

            byte[] fileContent = context.FileContent;
            var fileStream = new MemoryStream(fileContent);

            Workbook workbook = new Workbook(fileStream);
            Vanrise.Common.Utilities.ActivateAspose();
            Worksheet worksheet = workbook.Worksheets[0];

            var cdrs = new List<CDR>();
            Dictionary<string, PropertyInfo> cdrProperties = GetCDRProperties();

            for (int rowIndex = this.FirstRowIndex; rowIndex < worksheet.Cells.Rows.Count; rowIndex++)
            {
                var cdr = new CDR();
                int numberOfEmptyCells = 0;

                foreach (ExcelFileFieldMapping fldMapping in this.FieldMappings)
                {
                    Cell cell = worksheet.Cells[rowIndex, fldMapping.FieldIndex];
                    if (cell == null)
                        throw new NullReferenceException(String.Format("cell[{0}, {1}]", rowIndex, fldMapping.FieldIndex));

                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        numberOfEmptyCells++;
                        continue;
                    }

                    object cellValue = cell.Value;
                    PropertyInfo prop;
                    if (cdrProperties.TryGetValue(fldMapping.FieldName, out prop))
                        prop.SetValue(cdr, ConvertValue(cellValue, prop.PropertyType));
                    else
                        cdr.ExtraFields.Add(fldMapping.FieldName, cellValue);
                }

                if (numberOfEmptyCells == this.FieldMappings.Count)
                    continue;

                cdrs.Add(cdr);
                if (cdrs.Count == 100000)
                {
                    context.OnCDRsReceived(cdrs);
                    cdrs = new List<CDR>();
                }
            }

            if (cdrs.Count > 0)
                context.OnCDRsReceived(cdrs);
        }

        public override CDRSample ReadSample(IReadSampleFromFileContext context)
        {
            var sample = new ExcelFileCDRSample();
            var rows = new List<ExcelFileDataRow>();

            byte[] fileContent = context.FileContent;
            var fileStream = new MemoryStream(fileContent);

            Vanrise.Common.Utilities.ActivateAspose();

            Workbook workbook;
            Worksheet worksheet;

            try
            {
                workbook = new Workbook(fileStream);
                worksheet = workbook.Worksheets[0];
                if (worksheet.Cells.MaxDataColumn == -1)
                {
                    sample.ColumnCount = 0;
                    sample.ErrorMessage = "Excel file is blank";
                    return sample;
                }
                sample.ColumnCount = worksheet.Cells.MaxDataColumn + 1;
            }
            catch
            {
                sample.ColumnCount = 0;
                sample.ErrorMessage = "Invalid excel file";
                return sample;
            }

            int numberOfSampleRows = 0;
            int targetNumberOfSampleRows = 10;

            for (int rowIndex = 0; rowIndex < worksheet.Cells.Rows.Count && numberOfSampleRows < targetNumberOfSampleRows; rowIndex++)
            {
                var data = new List<string>();
                Aspose.Cells.Row row = worksheet.Cells.Rows[rowIndex];

                if (row.LastCell == null)
                    continue;

                int numberOfEmptyCells = 0;

                for (int columnIndex = 0; columnIndex <= row.LastCell.Column; columnIndex++)
                {
                    Cell cell = worksheet.Cells[rowIndex, columnIndex];
                    if (cell == null)
                        throw new NullReferenceException(String.Format("cell[{0}, {1}]", rowIndex, columnIndex));

                    if (string.IsNullOrWhiteSpace(cell.StringValue))
                    {
                        numberOfEmptyCells++;
                    }

                    data.Add(cell.StringValue);
                }

                if (numberOfEmptyCells < data.Count)
                {
                    rows.Add(new ExcelFileDataRow() { Data = data });
                    numberOfSampleRows++;
                }
            }

            sample.Rows = rows;
            return sample;
        }

        #endregion

        #region Private Methods

        private object ConvertValue(object fldValue, Type propertyType)
        {
            if (fldValue == null)
                return null;
            else if (propertyType == typeof(string))
                return fldValue.ToString();
            else if (propertyType == typeof(DateTime))
            {
                if (fldValue is DateTime)
                {
                    return fldValue;
                }
                DateTime dateTimeValue;
                if (!DateTime.TryParseExact(fldValue.ToString(), this.DateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeValue))
                    throw new Exception(String.Format("Invalid date time format '{0}'. Could not parse date time value '{1}'", this.DateTimeFormat, fldValue));
                return dateTimeValue;
            }
            else if (propertyType == typeof(TimeSpan))
                return (fldValue is TimeSpan) ? fldValue : TimeSpan.Parse(fldValue.ToString());
            else if (propertyType == typeof(Boolean))
                return (fldValue is Boolean) ? fldValue : Boolean.Parse(fldValue.ToString());
            else
                return Convert.ChangeType(fldValue, propertyType, NumberFormatInfo.CurrentInfo);
        }

        private Dictionary<string, PropertyInfo> GetCDRProperties()
        {
            Dictionary<string, PropertyInfo> cdrProperties = new Dictionary<string, PropertyInfo>();
            foreach (var prop in typeof(CDR).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                cdrProperties.Add(prop.Name, prop);
            }
            return cdrProperties;
        }

        #endregion
    }

    public class ExcelFileFieldMapping
    {
        public int FieldIndex { get; set; }
        public string FieldName { get; set; }
    }
}
