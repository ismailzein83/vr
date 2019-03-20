using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public class DataSourceSettingData : SettingData
    {
        public FileDataSourceSettings FileDataSourceSettings { get; set; }

        public override bool IsValid(ISettingDataValidationContext context)
        {
            if (FileDataSourceSettings == null)
            {
                context.ErrorMessage = "Data source settings is empty";
                return false;
            }

            StringBuilder errorMessages = new StringBuilder();

            if (FileDataSourceSettings.FileDataSourceDefinitions == null || FileDataSourceSettings.FileDataSourceDefinitions.Count == 0)
                errorMessages.AppendLine("File Import Exception Setting is empty");
            else if (FileDataSourceSettings.FileDataSourceDefinitions.Any(item => string.IsNullOrEmpty(item.Name)))
                errorMessages.AppendLine("File Import Exception Setting is invalid");

            if (FileDataSourceSettings.PeakTimeRanges != null && FileDataSourceSettings.PeakTimeRanges.Count > 0 && FileDataSourceSettings.PeakTimeRanges.Any(item => item.From == null || item.To == null))
                errorMessages.AppendLine("Peak Time Ranges don't have valid values");

            if (errorMessages.Length == 0)
                return true;

            context.ErrorMessage = errorMessages.ToString();
            return false;
        }
    }

    public class FileDataSourceSettings
    {
        public List<PeakTimeRange> PeakTimeRanges { get; set; }

        public List<FileDataSourceDefinition> FileDataSourceDefinitions { get; set; }
    }

    public class PeakTimeRange
    {
        public Guid PeakTimeRangeId { get; set; }

        public Time From { get; set; }

        public Time To { get; set; }
    }
}