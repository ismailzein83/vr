using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.MainExtensions.CDRFileReaders
{
    public class FlatFileReader : CDRFileReader
    {
        #region Properties

        public char Delimiter { get; set; }

        public List<FlatFileFieldMapping> FieldMappings { get; set; }

        public string DateTimeFormat { get; set; }

        #endregion

        #region Override

        public override void ReadCDRs(IReadCDRsFromFileContext context)
        {
            if (this.FieldMappings == null)
                throw new ArgumentNullException("FieldMappings");
            Dictionary<string, PropertyInfo> cdrProperties = GetCDRProperties();
            List<CDR> cdrs = new List<CDR>();

            string line;
            while (context.TryReadLine(out line))
            {
                string[] fields = line.Split(this.Delimiter);
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
            }
            context.OnCDRsReceived(cdrs);
        }

        #endregion

        #region Private Methods

        private object ConvertValue(string fldValue, Type propertyType)
        {
            if (propertyType == typeof(string))
                return fldValue;
            else if (propertyType == typeof(DateTime))
                return DateTime.ParseExact(fldValue, this.DateTimeFormat, CultureInfo.CurrentCulture);
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
