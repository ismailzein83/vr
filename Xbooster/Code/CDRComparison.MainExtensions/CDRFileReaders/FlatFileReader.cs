using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.MainExtensions
{
    public class FlatFileReader : CDRFileReader
    {
        public override Guid ConfigId { get { return new Guid("ff8e5a92-6404-4f69-b62d-1537daecb184"); } }

        #region Properties

        public char? Delimiter { get; set; }

        public bool IsTabDelimited { get; set; }

        public List<FlatFileFieldMapping> FieldMappings { get; set; }

        public string DateTimeFormat { get; set; }

        public int FirstRowIndex { get; set; }

        #endregion

        #region Override

        public override void ReadCDRs(IReadCDRsFromFileContext context)
        {
            if (this.FieldMappings == null)
                throw new ArgumentNullException("FieldMappings");
            Dictionary<string, PropertyInfo> cdrProperties = GetCDRProperties();
            List<CDR> cdrs = new List<CDR>();

            string line;
            if (this.FirstRowIndex > 0)
            {
                for (int i = 0; i < this.FirstRowIndex; i++)
                    context.TryReadLine(out line);
            }

            char delimiter = (this.IsTabDelimited) ? '\t' : this.Delimiter.Value;

            while (context.TryReadLine(out line))
            {
                string[] fields = line.Split(delimiter);
                var cdr = new CDR
                {
                    ExtraFields = new Dictionary<string, object>()
                };
                foreach (var fldMapping in this.FieldMappings)
                {
                    if (fldMapping.FieldIndex >= fields.Length)
                        throw new Exception(String.Format("Number of fields {0} is less than required. requested field mapping index is {1}", fields.Length, fldMapping.FieldIndex));
                    string fldValue = fields[fldMapping.FieldIndex];
                    PropertyInfo prop;
                    if (cdrProperties.TryGetValue(fldMapping.FieldName, out prop))
                        prop.SetValue(cdr, ConvertValue(fldValue, prop.PropertyType));
                    else
                        cdr.ExtraFields.Add(fldMapping.FieldName, fldValue);
                }
                cdrs.Add(cdr);
                if(cdrs.Count == 100000)
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
            var sample = new FlatFileCDRSample();
            var rows = new List<FlatFileDataRow>();

            string line;
            int counter = 0;

            char delimiter = (this.IsTabDelimited) ? '\t' : this.Delimiter.Value;

            while (context.TryReadLine(out line) && counter < 10)
            {
                string[] data = line.Split(delimiter);

                if (counter == 0)
                    sample.ColumnCount = data.Length;
                else if (data.Length != sample.ColumnCount)
                {
                    sample.ColumnCount = 0;
                    sample.ErrorMessage = String.Format("Invalid file format", counter + 1);
                    return sample;
                }

                rows.Add(new FlatFileDataRow() { Data = data });
                counter++;
            }

            sample.Rows = rows;
            return sample;
        }

        #endregion

        #region Private Methods

        private object ConvertValue(string fldValue, Type propertyType)
        {
            if (propertyType == typeof(string))
                return fldValue;
            else if (propertyType == typeof(DateTime)) {
                DateTime dateTimeValue;
                if (!DateTime.TryParseExact(fldValue, this.DateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeValue))
                    throw new Exception(String.Format("Invalid date time format '{0}'. Could not parse date time value '{1}'", this.DateTimeFormat, fldValue));
                return dateTimeValue;
            }
            else if (propertyType == typeof(TimeSpan))
                return TimeSpan.Parse(fldValue);
            else if (propertyType == typeof(Boolean))
                return Boolean.Parse(fldValue);
            else return Convert.ChangeType(fldValue, propertyType, NumberFormatInfo.CurrentInfo);
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

    public class FlatFileFieldMapping
    {
        public int FieldIndex { get; set; }

        public string FieldName { get; set; }
    }
}
